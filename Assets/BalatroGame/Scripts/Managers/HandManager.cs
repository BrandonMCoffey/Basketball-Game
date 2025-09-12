using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;

public class HandManager : DraggableContainer
{
    public static event Action<int> OnDeckCountChanged;
    public static HandManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RectTransform _deckTransform;
    [SerializeField] private RectTransform _discardTransform;

    [Header("Hand Settings")]
    [SerializeField] private int _maxSelectedCards = 5;
    [SerializeField] private int _handSize = 8;

    [Header("Hand Layout")]
    [SerializeField] private float _handWidth = 800f;
    [SerializeField] private float _arcPower = 150f;
    [SerializeField] private float _maxCardRotation = 15f;
    [SerializeField] private float _cardSpacing = 10f;

    public List<CardUI> SelectedCards => _selectedCards;

    private List<CardUI> _selectedCards = new List<CardUI>();
    private List<string> _deckCardIDs = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetupDeck();
    }

    private void SetupDeck()
    {
        _deckCardIDs = DataManager.Instance.AllCards.Keys.ToList();

        var rnd = new System.Random();
        _deckCardIDs = _deckCardIDs.OrderBy(item => rnd.Next()).ToList();
        OnDeckCountChanged?.Invoke(_deckCardIDs.Count);
    }

    public void ResetAndShuffleDeck()
    {
        foreach (DraggableUIItem card in _items)
        {
            Destroy(card.gameObject);
        }
        _items.Clear();
        _selectedCards.Clear();

        SetupDeck();

        StartCoroutine(DrawCardsRoutine(_handSize));
    }

    public IEnumerator DrawCardsRoutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (_deckCardIDs.Count == 0)
            {
                Debug.Log("Deck is empty!");
                break;
            }

            string id = _deckCardIDs[0];
            _deckCardIDs.RemoveAt(0);
            OnDeckCountChanged?.Invoke(_deckCardIDs.Count);

            CardData data = DataManager.Instance.AllCards[id];
            GameObject cardObject = Instantiate(_cardPrefab, transform);
            cardObject.transform.position = _deckTransform.position;

            CardUI cardUI = cardObject.GetComponent<CardUI>();
            cardUI.Initialize(data, this);
            _items.Add(cardUI);
        }
        yield return new WaitForSeconds(0.2f);
        UpdateLayout(true);
    }

    public IEnumerator AnimateDiscardRoutine(List<CardUI> cardsToRemove)
    {
        var tweens = new List<Tween>();
        foreach (var card in cardsToRemove)
        {
            card.DisableInteraction();
            _items.Remove(card);

            Tween moveTween = card.transform.DOMove(_discardTransform.position, 0.5f).SetEase(Ease.InBack);
            card.transform.DOScale(Vector3.zero, 0.5f);
            tweens.Add(moveTween);
        }

        if (tweens.Count > 0)
        {
            yield return tweens[0].WaitForCompletion();
        }

        foreach (var card in cardsToRemove)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        UpdateLayout(false);
    }

    protected override void UpdateLayout(bool animated)
    {
        int cardCount = _items.Count;
        if (cardCount == 0) return;

        float totalWidth = _handWidth + ((cardCount - 1) * _cardSpacing);

        for (int i = 0; i < cardCount; i++)
        {
            CardUI card = _items[i] as CardUI;
            if (card == null) continue;

            float t = (cardCount > 1) ? (float)i / (cardCount - 1) - 0.5f : 0;
            float xPos = t * totalWidth;
            float yPos = -Mathf.Abs(t) * Mathf.Abs(t) * _arcPower;
            Vector3 targetPosition = new Vector3(xPos, yPos, 0);
            Quaternion targetRotation = Quaternion.Euler(0, 0, -t * _maxCardRotation);

            if (animated)
            {
                card.SetHomePose(targetPosition, targetRotation);
            }
            else
            {
                card.transform.localPosition = targetPosition;
                card.transform.localRotation = targetRotation;
            }
        }
    }

    public void ToggleCardSelection(CardUI card)
    {
        bool isSelected = _selectedCards.Contains(card);

        if (isSelected)
        {
            _selectedCards.Remove(card);
            card.ToggleSelectionVisual(false);
        }
        else
        {
            if (_selectedCards.Count < _maxSelectedCards)
            {
                _selectedCards.Add(card);
                card.ToggleSelectionVisual(true);
            }
        }
    }
}