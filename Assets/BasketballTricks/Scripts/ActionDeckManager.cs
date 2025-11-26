using DG.Tweening;
using SaiUtils.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionDeckManager : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private ActionCard _cardPrefab;
    [SerializeField] private int _handSize = 5;
    [SerializeField] private Transform _cardContainer;
    //[SerializeField] private RectTransform _selectionBoxVisual;

    [Header("Arc Layout")]
    [SerializeField] private float _arcRadius = 800f;
    [SerializeField] private float _arcAngleSpread = 40f;
    [SerializeField] private Vector2 _centerOffset = new Vector2(0, -600);
    [SerializeField] private float _selectedHeightOffset = 100f;

    [Header("Animation")]
    [SerializeField] private float _layoutDuration = 0.3f;
    [SerializeField] private Ease _layoutEase = Ease.OutBack;

    private List<ActionCard> _cards = new List<ActionCard>();
    private List<ActionDeckCard> _actionDeck = new List<ActionDeckCard>();
    
    //private Vector2 _startDragPos;
    //private RectTransform _selectionRectTr;

    public void Init()
    {
        var players = PlayerManager.Instance.Players;
        _actionDeck = new List<ActionDeckCard>();
        foreach (var player in players)
        {
            for (int i = 0; i < player.CardData.ActionCount; i++)
            {
                _actionDeck.Add(new ActionDeckCard { Player = player, ActionIndex = i });
            }
        }
        _actionDeck.Shuffle();

        int count = Mathf.Min(_handSize, _actionDeck.Count);
        _cards = new List<ActionCard>(count);
        for (int i = 0; i < count; i++)
        {
            var actionCard = Instantiate(_cardPrefab, _cardContainer, false);
            var pos = actionCard.transform.position;
            pos.x = _cardContainer.position.x + (i - 2) * 300f;
            actionCard.transform.position = pos;
            actionCard.Init(_actionDeck[i].Player.CardData.GetAction(_actionDeck[i].ActionIndex), _actionDeck[i].Player.PositionColor, this);
            _cards.Add(actionCard);
        }
        //_selectionRectTr = _selectionBoxVisual;
        //_selectionBoxVisual.gameObject.SetActive(false);
    }

    public void UpdateCardLayout(ActionCard draggingCard = null)
    {
        int count = _cards.Count;
        float angleStep = count > 1 ? _arcAngleSpread / (count - 1) : 0;
        float currentAngle = -_arcAngleSpread / 2;

        for (int i = 0; i < count; i++)
        {
            ActionCard card = _cards[i];

            // 1. Calculate Arc Position
            float rad = (currentAngle + 90) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * _arcRadius,
                Mathf.Sin(rad) * _arcRadius
            ) + _centerOffset;

            // 2. Add Selection Offset
            if (card.IsSelected && card != draggingCard)
            {
                pos += Vector2.up * _selectedHeightOffset;
            }

            Quaternion rot = Quaternion.Euler(0, 0, currentAngle);

            // 3. Apply
            if (card != draggingCard)
            {
                card.UpdateTransform(pos, rot, _layoutDuration, _layoutEase);
            }
            else
            {
                card.UpdateRotationOnly(rot, 0.1f);
            }

            currentAngle += angleStep;
        }
    }

    public void OnCardDragReorder(ActionCard draggedCard)
    {
        int draggedIndex = _cards.IndexOf(draggedCard);
        int newIndex = draggedIndex;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (i == draggedIndex) continue;
            if (draggedCard.transform.localPosition.x > _cards[i].transform.localPosition.x && draggedIndex < i) newIndex = i;
            else if (draggedCard.transform.localPosition.x < _cards[i].transform.localPosition.x && draggedIndex > i) newIndex = i;
        }

        if (newIndex != draggedIndex)
        {
            _cards.RemoveAt(draggedIndex);
            _cards.Insert(newIndex, draggedCard);
            UpdateCardLayout(draggedCard);
        }
    }

    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (var card in _cards) card.SetSelected(false);
        UpdateCardLayout();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _selectionBoxVisual.gameObject.SetActive(true);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_cardContainer as RectTransform, eventData.position, eventData.pressEventCamera, out _startDragPos);
        UpdateSelectionBox(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateSelectionBox(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _selectionBoxVisual.gameObject.SetActive(false);
        ApplyBoxSelection();
    }

    private void UpdateSelectionBox(PointerEventData eventData)
    {
        Vector2 currentPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_cardContainer as RectTransform, eventData.position, eventData.pressEventCamera, out currentPos);

        float width = currentPos.x - _startDragPos.x;
        float height = currentPos.y - _startDragPos.y;

        _selectionRectTr.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        _selectionRectTr.anchoredPosition = _startDragPos + new Vector2(width / 2, height / 2);
    }

    private void ApplyBoxSelection()
    {
        Vector3[] corners = new Vector3[4];
        _selectionBoxVisual.GetWorldCorners(corners);
        Rect selectionRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

        foreach (var card in _cards)
        {
            Vector3[] cardCorners = new Vector3[4];
            ((RectTransform)card.transform).GetWorldCorners(cardCorners);
            Vector2 cardCenter = (cardCorners[0] + cardCorners[2]) / 2;

            if (selectionRect.Contains(cardCenter))
            {
                card.SetSelected(true);
            }
        }
        UpdateCardLayout();
    }
    */
}

public struct ActionDeckCard
{
    public Player Player;
    public int ActionIndex;
}