using DG.Tweening;
using SaiUtils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionDeckManager : MonoBehaviour
{
    [SerializeField] private ActionCard _cardPrefab;
    [SerializeField] private int _cardsDrawnPerHand = 5;
    [SerializeField] private int _maxHandSize = 9;
    [SerializeField] private RectTransform _cardContainer;
    [SerializeField] private RectTransform _cardPlayPoint;
    [SerializeField] private RectTransform _cardRemovedPlayedPoint;
    [SerializeField] private RectTransform _discardBox;
    [SerializeField] private float _dragReorderThreshold = 0.3f;

    [Header("Play Preview")]
    [SerializeField] private RectTransform _playPreviewStart;
    [SerializeField] private float _playPreviewScale = 0.2f;

    [Header("Card Layout")]
    [SerializeField] private RectTransform _cardSpread;
    [SerializeField] private float _maxCardSpacing = 20f;
    [SerializeField] private float _verticalSpread = 50f;
    [SerializeField] private float _maxRotationAngle = 30f;
    [SerializeField] private float _selectedHeightOffset = 100f;
    [SerializeField] private float _layoutDuration = 0.3f;
    [SerializeField] private Ease _layoutEase = Ease.OutBack;
    [SerializeField] RectTransform _playButton;

    [Header("Goal Values")]
    [SerializeField] HypeScoreDisplay _hypeScoreDisplay;
    [SerializeField, Range(0, 350)] private float _goalValue = 0.25f;
    [SerializeField, Range(0, 350)] private float _bronzeValue = 0.25f;
    [SerializeField, Range(0, 350)] private float _silverValue = 0.25f;
    [SerializeField, Range(0, 350)] private float _goldValue = 0.25f;
    public static float MaxScore {get; private set; } = 350f;

    public event System.Action BeforeDrawingNextHand = delegate { };
    public static event System.Action<ActionCard> OnCardPlayed = delegate { };
    public static event System.Action OnPlayPressed = delegate { };
    public static event System.Action OnSequenceEnd = delegate { };
    public RectTransform DiscardBox => _discardBox;
    public RectTransform CardPlayPoint => _cardPlayPoint;
    public RectTransform CardRemovedPlayedPoint => _cardRemovedPlayedPoint;

    private List<ActionCard> _cards = new List<ActionCard>();
    private List<ActionCard> _playedCards = new List<ActionCard>();
    private List<GameAction> _actionDeck = new List<GameAction>();
    private bool _disabled;
    private bool _lockReorder;
    private bool _discardShown;

    private void Awake()
    {
        if (_discardBox != null) _discardBox.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerManager.RefreshTimeline += CheckSequenceCompleted;
    }

    private void OnDisable()
    {
        PlayerManager.RefreshTimeline -= CheckSequenceCompleted;
    }

    private void Update()
    {
        if (_discardShown != ActionCard.DraggingAny)
        {
            _discardShown = ActionCard.DraggingAny;
            if (_discardBox != null) _discardBox.gameObject.SetActive(_discardShown);
        }
    }

    public void Init()
    {
        var players = PlayerManager.Instance.Players;
        foreach (var player in players)
        {
            for (int i = 0; i < player.CardData.ActionCount; i++)
            {
                if (player.CardData.GetAction(i).AllowedPositions.HasFlag(player.Position))
                {
                    int count = player.CardData.GetActionCount(i);
                    for (int j = 0; j < count; j++)
                    {
                        _actionDeck.Add(new GameAction(player, i));
                    }
                }
            }
        }
        _actionDeck.Shuffle();

        for (int i = _cardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_cardContainer.GetChild(i).gameObject);
        }
        DrawHand();
        UpdateCardLayout(null, true);

        _hypeScoreDisplay.Init(_goalValue, _bronzeValue, _silverValue, _goldValue);
    }

    public void InitTutorial(List<GameAction> deck, bool init)
    {
        _actionDeck = new List<GameAction>(deck);
        for (int i = _cards.Count - 1; i >= 0; i--)
        {
            Destroy(_cards[i].gameObject);
        }
        _cards.Clear();
        if (init)
        {
            for (int i = _cardContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }
            DrawHand();
            UpdateCardLayout(null, true);
        }
    }

    private void DrawHand()
    {
        int drawCount = Mathf.Min(Mathf.Min(_cardsDrawnPerHand, _maxHandSize - _playedCards.Count), _actionDeck.Count);
        for (int i = 0; i < drawCount; i++)
        {
            ActionCard card;
            if (_playedCards.Count > 0)
            {
                card = _playedCards[0];
                _playedCards.RemoveAt(0);
            }
            else
            {
                card = Instantiate(_cardPrefab, _cardContainer);
            }
            card.Init(_actionDeck[0], this);
            _actionDeck.RemoveAt(0);
            _cards.Add(card);
            Debug.Log($"Drew card: {card.Action.Player.CardData.GetAction(card.Action.ActionIndex).Name}");
        }
        for (int i = _playedCards.Count - 1; i >= 0; i--)
        {
            Destroy(_playedCards[i].gameObject);
        }
        _playedCards.Clear();
        PlayerManager.Instance.PreviewSequence(_playedCards, _cards);
    }

    public void PlayCard(ActionCard card)
    {
        OnCardPlayed?.Invoke(card);
        _cards.Remove(card);
        _playedCards.Add(card);
        PlayerManager.Instance.PreviewSequence(_playedCards, _cards);
        UpdateCardLayout(null);
    }

    public void RemoveCardFromPlay(ActionCard card)
    {
        _playedCards.Remove(card);
        _cards.Add(card);
        PlayerManager.Instance.PreviewSequence(_playedCards, _cards);
        UpdateCardLayout(null);
    }

    public void OnUpdateSelected()
    {
        if (_disabled) return;
        //PlayerManager.Instance.PreviewSequence(_cards.Where(card => card.IsSelected).ToList());
    }

    public void DiscardCard(ActionCard card) => StartCoroutine(DiscardCardRoutine(card));
    private IEnumerator DiscardCardRoutine(ActionCard card)
    {
        card.RectTransform.DOAnchorPos(card.RectTransform.anchoredPosition + Vector2.down * 1080f, _layoutDuration).SetEase(Ease.InBack);
        yield return new WaitForSeconds(_layoutDuration + 0.1f);
        if (_actionDeck.Count > 0)
        {
            card.Init(_actionDeck[0], this);
            _actionDeck.RemoveAt(0);
            PlayerManager.Instance.PreviewSequence(_playedCards, _cards);
            UpdateCardLayout(null);
        }
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
            foreach (var card in _playedCards)
            {
                var t = card.RectTransform;
                t.DOAnchorPos(t.anchoredPosition + Vector2.up * 500, 1f).SetEase(Ease.InBack).OnComplete(() => {
                    t.anchoredPosition = new Vector2(0f, -1080f);
                });
            }
        }

        OnPlayPressed?.Invoke();
        _playButton.gameObject.SetActive(false);
    }

    private void CheckSequenceCompleted()
    {
        if (_disabled && !PlayerManager.Instance.Simulating)
        {
            _disabled = false;
            BeforeDrawingNextHand?.Invoke();
            DrawHand();
            UpdateCardLayout(null);
            OnSequenceEnd?.Invoke();
            _playButton.gameObject.SetActive(true);
        }
    }

    public void UpdateCardLayout(ActionCard draggingCard = null, bool immediate = false)
    {
        if (_disabled) return;
        int count = _cards.Count;
        if (count > 0)
        {
            float delta = count > 1 ? 1f / (count - 1) : 0f;

            Rect cardRect = _cards[0].RectTransform.rect;
            Vector2 center = _cardSpread.anchoredPosition;
            float horzSpread = Mathf.Min(_cardSpread.rect.width, count * (cardRect.width + _maxCardSpacing)) * 0.5f;
            float vertSpread = cardRect.height * _verticalSpread * 0.5f;
            for (int i = 0; i < count; i++)
            {
                ActionCard card = _cards[i];
                float vertDelta = Mathf.Abs(2f * delta * i - 1f);
                vertDelta *= vertDelta;
                Vector2 pos = center + new Vector2(Mathf.Lerp(-horzSpread, horzSpread, delta * i), Mathf.Lerp(vertSpread, -vertSpread, vertDelta));
                if (card.IsSelected && card != draggingCard) pos.y += _selectedHeightOffset;

                Quaternion rot = Quaternion.Euler(0f, 0f, Mathf.Lerp(_maxRotationAngle, -_maxRotationAngle, delta * i));

                if (card != draggingCard)
                {
                    card.UpdateTransform(pos, rot, immediate ? 0 : _layoutDuration, _layoutEase);
                }
            }
        }
        count = _playedCards.Count;
        if (count > 0)
        {
            float cardWidth = _playedCards[0].RectTransform.rect.width;
            for (int i = 0; i < count; i++)
            {
                var pos = _playPreviewStart.anchoredPosition + (cardWidth * _playPreviewScale + 20f) * i * Vector2.right;
                if (_playedCards[i] != draggingCard)
                {
                    _playedCards[i].RectTransform.DOAnchorPos(pos, immediate ? 0 : 0.2f);
                    _playedCards[i].RectTransform.DOScale(_playPreviewScale, immediate ? 0 : 0.2f);
                }
            }
        }
    }

    public void OnCardDragReorder(ActionCard draggedCard, bool forceIfLocked = false)
    {
        if (_lockReorder && !forceIfLocked) return;
        int count = _cards.Count;
        if (_disabled || count <= 1) return;

        int draggedIndex = _cards.IndexOf(draggedCard);
        //Debug.Log($"Dragged index: {draggedIndex}");
        int newIndex = draggedIndex;

        float delta = 1f / (count - 1);

        Rect cardRect = _cards[0].RectTransform.rect;
        Vector2 center = _cardSpread.anchoredPosition;
        float horzSpread = Mathf.Min(_cardSpread.rect.width, count * (cardRect.width + _maxCardSpacing)) * 0.5f;
        float vertSpread = cardRect.height * _verticalSpread * 0.5f;

        float draggedX = draggedCard.RectTransform.anchoredPosition.x;
        for (int i = 0; i < count; i++)
        {
            if (i == draggedIndex) continue;

            float x = center.x + Mathf.Lerp(-horzSpread, horzSpread, delta * i);
            float threshold = Mathf.Min(_dragReorderThreshold * cardRect.width, horzSpread * delta - 10f);

            //Debug.Log($"{draggedIndex}: {draggedCard.RectTransform.anchoredPosition.x} vs {i}: {x}, {threshold}");
            if (draggedX > x - threshold && newIndex < i) newIndex = i;
            else if (draggedX < x + threshold && newIndex > i) newIndex = i;
        }

        if (newIndex != draggedIndex && draggedIndex >= 0 && draggedIndex < _cards.Count)
        {
            _cards.RemoveAt(draggedIndex);
            _cards.Insert(newIndex, draggedCard);
            if (!forceIfLocked)
            {
                UpdateCardLayout(draggedCard);
                if (_cards.Any(c => c.IsSelected)) OnUpdateSelected();
                StartCoroutine(LockReorderTemp());
            }
        }
    }

    public void OnCardDragReorderPlayed(ActionCard draggedCard, bool forceIfLocked = false)
    {
        if (_lockReorder && !forceIfLocked) return;
        int count = _playedCards.Count;
        if (_disabled || count <= 1) return;

        int draggedIndex = _playedCards.IndexOf(draggedCard);
        int newIndex = draggedIndex;

        float cardWidth = _playedCards[0].RectTransform.rect.width;
        float draggedX = draggedCard.RectTransform.anchoredPosition.x;
        for (int i = 0; i < count; i++)
        {
            if (i == draggedIndex) continue;

            float x = _playPreviewStart.anchoredPosition.x + (cardWidth * _playPreviewScale + 20f) * i;
            float threshold = _dragReorderThreshold * cardWidth * 0.5f;

            //Debug.Log($"{draggedIndex}: {draggedCard.RectTransform.anchoredPosition.x} vs {i}: {x}, {threshold}");
            if (draggedX > x - threshold && newIndex < i) newIndex = i;
            else if (draggedX < x + threshold && newIndex > i) newIndex = i;
        }

        if (newIndex != draggedIndex)
        {
            _playedCards.RemoveAt(draggedIndex);
            _playedCards.Insert(newIndex, draggedCard);
            PlayerManager.Instance.PreviewSequence(_playedCards, _cards);
            if (!forceIfLocked)
            {
                UpdateCardLayout(draggedCard);
                StartCoroutine(LockReorderTemp());
            }
        }
    }

    private IEnumerator LockReorderTemp()
    {
        _lockReorder = true;
        yield return new WaitForSeconds(0.1f);
        _lockReorder = false;
    }
}