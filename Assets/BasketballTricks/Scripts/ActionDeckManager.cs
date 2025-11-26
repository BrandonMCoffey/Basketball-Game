using DG.Tweening;
using SaiUtils.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class ActionDeckManager : MonoBehaviour
{
    [SerializeField] private ActionCard _cardPrefab;
    [SerializeField] private int _handSize = 5;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private float _dragReorderThreshold = 50f;

    [Header("Card Layout")]
    [SerializeField, Range(0f, 1f)] private float _horizontalScreenPercent = 0.7f;
    [SerializeField] private float _verticalSpread = 50f;
    [SerializeField] private float _maxRotationAngle = 30f;
    [SerializeField] private float _selectedHeightOffset = 100f;
    [SerializeField] private float _layoutDuration = 0.3f;
    [SerializeField] private Ease _layoutEase = Ease.OutBack;

    private List<ActionCard> _cards;
    private List<ActionDeckCard> _actionDeck;

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
            actionCard.Init(_actionDeck[i].Player.CardData.GetAction(_actionDeck[i].ActionIndex), _actionDeck[i].Player.PositionColor, this);
            _cards.Add(actionCard);
        }
        UpdateCardLayout(null);
    }

    public void UpdateCardLayout(ActionCard draggingCard = null)
    {
        int count = _cards.Count;
        float delta = count > 1 ? 1f / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            ActionCard card = _cards[i];

            float horz = Screen.width * _horizontalScreenPercent * 0.5f;
            float vert = _verticalSpread * 0.5f;
            float vertDelta = Mathf.Abs(2f * delta * i - 1f);
            vertDelta *= vertDelta;
            Vector2 pos = new Vector2(Mathf.Lerp(-horz, horz, delta * i), Mathf.Lerp(vert, -vert, vertDelta));
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
        if (count <= 1) return;

        int draggedIndex = _cards.IndexOf(draggedCard);
        int newIndex = draggedIndex;

        float delta = 1f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            if (i == draggedIndex) continue;

            float horz = Screen.width * _horizontalScreenPercent * 0.5f;
            float x = Mathf.Lerp(-horz, horz, delta * i);
            float threshold = Mathf.Min(_dragReorderThreshold, horz * delta - 10f);
            
            if (draggedCard.transform.localPosition.x > x - threshold && draggedIndex < i) newIndex = i;
            else if (draggedCard.transform.localPosition.x < x + threshold && draggedIndex > i) newIndex = i;
        }

        if (newIndex != draggedIndex)
        {
            _cards.RemoveAt(draggedIndex);
            _cards.Insert(newIndex, draggedCard);
            UpdateCardLayout(draggedCard);
        }
    }
}

public struct ActionDeckCard
{
    public Player Player;
    public int ActionIndex;
}