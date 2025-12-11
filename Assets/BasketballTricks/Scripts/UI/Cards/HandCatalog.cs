using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HandCatalog : MonoBehaviour
{
    [SerializeField] private PlayerCard _cardPrefab;
    [SerializeField] private Transform _cardParent;
    [SerializeField] private Transform _holdParent;
    [SerializeField] private bool _dragOntoCourt;
    [SerializeField] private Vector2 _cardSize = new Vector2(200, 300);
    [SerializeField] private float _spacing = 10;
    [SerializeField] private int _columns = 2;
    [SerializeField] private int _maxDisplayCards = 6;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private RectTransform _focusPoint;
    [SerializeField] private bool _showOnStart = true;
    [SerializeField] private float _betweenCardTime = 0.05f;
    [SerializeField] private float _animTime = 0.3f;

    private PlayerPosition _positionFilter = PlayerPosition.None;
    private int _startIndex;
    private List<GameCard> _allCards;
    private List<PlayerCard> _cards;
    private bool _transitioning;

    private void OnEnable()
    {
        GameManager.UpdatePlayerLoadout += RefreshCards;
    }

    private void OnDisable()
    {
        GameManager.UpdatePlayerLoadout -= RefreshCards;
    }

    private void Start()
    {
        _allCards = GameManager.Instance.OwnedPlayers;
        _startIndex = 0;
        _cards = new List<PlayerCard>(_maxDisplayCards);
        ResetCardPositions(true);
        if (_showOnStart) UpdateCardData();
        if (_prevButton != null) _prevButton.onClick.AddListener(PreviousPage);
        if (_nextButton != null) _nextButton.onClick.AddListener(NextPage);
    }

    private void OnValidate()
    {
        if (_holdParent == null) _holdParent = transform;
    }

    public void StartGame() => GameManager.Instance.StartGame();

    private void ResetCardPositions(bool init = false)
    {
        for (int i = 0; i < _maxDisplayCards; i++)
        {
            if (i >= _cards.Count) _cards.Add(Instantiate(_cardPrefab, _cardParent));
            var rectTransform = _cards[i].GetComponent<RectTransform>();
            int column = i % _columns;
            int row = i / _columns;
            float x = (column - 0.5f * _columns + 0.5f) * (_cardSize.x + _spacing);
            float y = (-row + 0.5f * _maxDisplayCards / _columns - 0.5f) * (_cardSize.y + _spacing);
            //Debug.Log($"{i}: {row}, {column}, {column - 0.5f * _columns + 0.5f}, {row - 0.5f * count / _columns + 0.5f}");
            rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector3.one * 0.5f;
            rectTransform.anchoredPosition = new Vector2(x, y + 20); // offset y for better centering - Sai
            rectTransform.sizeDelta = _cardSize;
            if (init)
            {
                _cards[i].Init(_cardParent, _holdParent, _dragOntoCourt, _focusPoint.anchoredPosition);
                rectTransform.localScale = Vector3.zero;
            }
        }
        PlayerCard.CurrentInvisibleCard = null;
    }

    private void RefreshCards()
    {
        var loadout = GameManager.Instance.GetPositionLoadout();
        foreach (var card in _cards)
        {
            card.SetInteractable(card.Card != null && !loadout.Contains(card.Card));
        }
    }

    public void ShowCardsFiltered(PlayerPosition positionFilter = PlayerPosition.None)
    {
        _startIndex = 0;
        _positionFilter = positionFilter;
        _allCards = GameManager.Instance.OwnedPlayers;
        // Sort by filter (filtered cards first)
        if (_positionFilter != PlayerPosition.None)
        {
            _allCards = _allCards.OrderBy(card => !card.PlayerData.IsNaturalPosition(_positionFilter)).ToList();
        }
        // Sort used cards to the end
        var loadout = GameManager.Instance.GetPositionLoadout();
        _allCards = _allCards.OrderBy(card => loadout.Contains(card)).ToList();
        //ResetCardPositions();
        foreach (var card in _cards)
        {
            card.AppearAtPosition();
        }
        UpdateCardData();
    }
    private void UpdateCardData()
    {
        var count = Mathf.Min(_allCards.Count - _startIndex, _maxDisplayCards);
        for (int i = 0; i < _cards.Count; i++)
        {
            int cardIndex = _startIndex + i;
            bool positionBonus = _positionFilter != PlayerPosition.None && _allCards[cardIndex].PlayerData.IsNaturalPosition(_positionFilter);
            if (i < count) _cards[i].SetData(_allCards[cardIndex], _positionFilter, positionBonus);
            else _cards[i].SetData(null);

            _cards[i].SetGlow(positionBonus);
        }
        StartCoroutine(ShowCardsRoutine(count));
    }

    public void HideCards()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.localScale = Vector3.zero;
        }
        UpdateInteractibility(true);
    }

    private IEnumerator ShowCardsRoutine(int cardsToShow)
    {
        if (_transitioning) yield return null;
        _transitioning = true;
        while (GameManager.InTransition) yield return null;
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.DOScale(i < cardsToShow ? Vector3.one : Vector3.zero, _animTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(_betweenCardTime);
        }
        UpdateInteractibility();
        _transitioning = false;
    }

    private void UpdateInteractibility(bool disable = false)
    {
        if (_prevButton != null) _prevButton.interactable = disable ? false : _startIndex > 0;
        if (_nextButton != null)
        {
            if (disable) _nextButton.interactable = false;
            else
            {
                int count = (GameManager.Instance.OwnedPlayers.Count - 1) / _maxDisplayCards * _maxDisplayCards;
                _nextButton.interactable = _startIndex < count;
            }
        }
        var loadout = GameManager.Instance.GetPositionLoadout();
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].SetInteractable(disable ? false : (_cards[i].Card != null && !loadout.Contains(_cards[i].Card)));
        }
    }

    public void NextPage()
    {
        int count = (GameManager.Instance.OwnedPlayers.Count - 1) / _maxDisplayCards * _maxDisplayCards;
        if (_startIndex == count) return;
        _startIndex = Mathf.Min(count, _startIndex + _maxDisplayCards);
        StartCoroutine(SwitchPageRoutine());
    }

    public void PreviousPage()
    {
        if (_startIndex == 0) return;
        _startIndex = Mathf.Max(0, _startIndex - _maxDisplayCards);
        StartCoroutine(SwitchPageRoutine());
    }

    private IEnumerator SwitchPageRoutine()
    {
        if (_transitioning) yield return null;
        _transitioning = true;
        UpdateInteractibility(true);
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.DOScale(Vector3.zero, _animTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(_betweenCardTime);
        }
        yield return new WaitForSeconds(_animTime); // Finish DoTween
        foreach (var card in _cards)
        {
            card.RefreshTransform();
            card.transform.localScale = Vector3.zero;
        }
        _transitioning = false;
        UpdateCardData();
    }
}