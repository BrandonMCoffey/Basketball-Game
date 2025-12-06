using DG.Tweening;
using SaiUtils.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionDeckManager : MonoBehaviour
{
    [SerializeField] private ActionCard _cardPrefab;
    [SerializeField] private int _handSize = 5;
    [SerializeField] private RectTransform _cardContainer;
    [SerializeField] private float _dragReorderThreshold = 50f;

    [Header("Card Layout")]
    [SerializeField] private float _horizontalSpread = 700f;
    [SerializeField] private float _verticalSpread = 50f;
    [SerializeField] private float _maxRotationAngle = 30f;
    [SerializeField] private float _selectedHeightOffset = 100f;
    [SerializeField] private float _layoutDuration = 0.3f;
    [SerializeField] private Ease _layoutEase = Ease.OutBack;

    private List<ActionCard> _cards;
    private List<GameAction> _actionDeck;
    private bool _disabled;

    private void OnEnable()
    {
        PlayerManager.RefreshTimeline += CheckSequenceCompleted;
    }

    private void OnDisable()
    {
        PlayerManager.RefreshTimeline -= CheckSequenceCompleted;
    }

    public void Init()
    {
        var players = PlayerManager.Instance.Players;
        _actionDeck = new List<GameAction>();
        foreach (var player in players)
        {
            for (int i = 0; i < player.CardData.ActionCount; i++)
            {
                _actionDeck.Add(new GameAction(player, i));
            }
        }
        _actionDeck.Shuffle();

        for (int i = _cardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_cardContainer.GetChild(i).gameObject);
        }

        int count = Mathf.Min(_handSize, _actionDeck.Count);
        _cards = new List<ActionCard>(count);
        float delta = count > 1 ? 1f / (count - 1) : 0f;
        for (int i = 0; i < count; i++)
        {
            float x = _cardContainer.localPosition.x + Mathf.Lerp(-_horizontalSpread, _horizontalSpread, delta * i);
            var actionCard = Instantiate(_cardPrefab, new Vector2(x, -Screen.height * 0.5f), Quaternion.identity, _cardContainer);
            actionCard.Init(_actionDeck[0], this);
            _actionDeck.RemoveAt(0);
            _cards.Add(actionCard);
        }
        UpdateCardLayout(null);
    }

    public void OnUpdateSelected()
    {
        if (_disabled) return;
        var sequence = _cards.Where(card => card.IsSelected).Select(card => card.Action).ToList();
        PlayerManager.Instance.PreviewSequence(sequence);
    }

    public void StartSequence()
    {
        if (_disabled) return;
        if (PlayerManager.Instance.RunSimulation())
        {
            _disabled = true;
            foreach (var card in _cards)
            {
                card.RectTransform.DOAnchorPos(card.RectTransform.anchoredPosition + Vector2.down * 1080f, 1f).SetEase(Ease.InBack);
            }
        }
    }

    private void CheckSequenceCompleted()
    {
        if (_disabled && !PlayerManager.Instance.Simulating)
        {
            _disabled = false;
            foreach (var card in _cards)
            {
                if (card.IsSelected)
                {
                    if (_actionDeck.Count > 0)
                    {
                        card.Init(_actionDeck[0], this);
                        _actionDeck.RemoveAt(0);
                    }
                    else
                    {
                        card.gameObject.SetActive(false);
                    }
                }
            }
            UpdateCardLayout(null);
        }
    }

    public void UpdateCardLayout(ActionCard draggingCard = null)
    {
        if (_disabled) return;
        int count = _cards.Count;
        float delta = count > 1 ? 1f / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            ActionCard card = _cards[i];

            float vert = _verticalSpread * 0.5f;
            float vertDelta = Mathf.Abs(2f * delta * i - 1f);
            vertDelta *= vertDelta;
            Vector2 pos = new Vector2(Mathf.Lerp(-_horizontalSpread, _horizontalSpread, delta * i), Mathf.Lerp(vert, -vert, vertDelta));
            if (card.IsSelected && card != draggingCard) pos.y += _selectedHeightOffset;

            Quaternion rot = Quaternion.Euler(0f, 0f, Mathf.Lerp(_maxRotationAngle, -_maxRotationAngle, delta * i));

            if (card != draggingCard)
            {
                card.UpdateTransform(pos, rot, _layoutDuration, _layoutEase);
            }
        }
    }

    public void OnCardDragReorder(ActionCard draggedCard)
    {
        int count = _cards.Count;
        if (_disabled || count <= 1) return;

        int draggedIndex = _cards.IndexOf(draggedCard);
        int newIndex = draggedIndex;

        float delta = 1f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            if (i == draggedIndex) continue;

            float x = Mathf.Lerp(-_horizontalSpread, _horizontalSpread, delta * i);
            float threshold = Mathf.Min(_dragReorderThreshold, _horizontalSpread * delta - 10f);
            
            if (draggedCard.transform.localPosition.x > x - threshold && draggedIndex < i) newIndex = i;
            else if (draggedCard.transform.localPosition.x < x + threshold && draggedIndex > i) newIndex = i;
        }

        if (newIndex != draggedIndex)
        {
            _cards.RemoveAt(draggedIndex);
            _cards.Insert(newIndex, draggedCard);
            UpdateCardLayout(draggedCard);
            if (_cards.Any(c => c.IsSelected)) OnUpdateSelected();
        }
    }
}