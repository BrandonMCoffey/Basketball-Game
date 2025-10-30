using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCatalog : MonoBehaviour
{
    [SerializeField] private PlayerCard _cardPrefab;
    [SerializeField] private Transform _cardParent;
    [SerializeField] private Vector2 _cardSize = new Vector2(200, 300);
    [SerializeField] private float _spacing = 10;
    [SerializeField, Range(1, 5)] private int _columns = 2;
    [SerializeField] private int _maxDisplayCards = 6;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private int _startIndex;
    [SerializeField] RectTransform _glareEffect;

    private List<PlayerCard> _cards;
    private bool _transitioning;
    private Coroutine _pageEffectsRoutine;

    private void Start()
    {
        CreateCards();
        if (_prevButton != null) _prevButton.onClick.AddListener(PreviousPage);
        if (_nextButton != null) _nextButton.onClick.AddListener(NextPage);

        ToggleAnimations(true);
    }

    public void StartGame() => GameManager.Instance.StartGame();

    private void CreateCards()
    {
        _cards = new List<PlayerCard>(_maxDisplayCards);
        for (int i = 0; i < _maxDisplayCards; i++)
        {
            PlayerCard card = Instantiate(_cardPrefab, _cardParent);
            var rectTransform = card.GetComponent<RectTransform>();
            int column = i % _columns;
            int row = i / _columns;
            float x = (column - 0.5f * _columns + 0.5f) * (_cardSize.x + _spacing);
            float y = (row - 0.5f * _maxDisplayCards / _columns + 0.5f) * (_cardSize.y + _spacing);
            //Debug.Log($"{i}: {row}, {column}, {column - 0.5f * _columns + 0.5f}, {row - 0.5f * count / _columns + 0.5f}");
            rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = Vector3.one * 0.5f;
            rectTransform.anchoredPosition = new Vector2(x, y + 20); // offset y for better centering - Sai
            rectTransform.sizeDelta = _cardSize;
            card.Init(_cardParent, transform);
            card.transform.localScale = Vector3.zero;
            _cards.Add(card);
        }
        UpdateInteractibility(true);
        UpdateCardData();
    }

    private void UpdateCardData()
    {
        var players = GameManager.Instance.Players;
        var count = Mathf.Min(players.Count - _startIndex, _maxDisplayCards);
        for (int i = 0; i < _cards.Count; i++)
        {
            int cardIndex = _startIndex + i;
            _cards[i].SetData(i < count ? players[cardIndex] : null);
        }
        StartCoroutine(ShowCardsRoutine(count));
    }

    private IEnumerator ShowCardsRoutine(int cardsToShow)
    {
        if (_transitioning) yield return null;
        _transitioning = true;
        while (GameManager.InTransition) yield return null;
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].transform.DOScale(i < cardsToShow ? Vector3.one : Vector3.zero, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
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
                int count = (GameManager.Instance.Players.Count - 1) / _maxDisplayCards * _maxDisplayCards;
                _nextButton.interactable = _startIndex < count;
            }
        }
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].SetInteractable(disable ? false : true);
        }
    }

    public void NextPage()
    {
        int count = (GameManager.Instance.Players.Count - 1) / _maxDisplayCards * _maxDisplayCards;
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
            _cards[i].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f); // Finish DoTween
        foreach (var card in _cards)
        {
            card.RefreshTransform();
            card.transform.localScale = Vector3.zero;
        }
        _transitioning = false;
        UpdateCardData();
    }

    public void ToggleAnimations(bool enabled)
    {
        if (_pageEffectsRoutine != null) StopCoroutine(_pageEffectsRoutine);
        if (enabled) _pageEffectsRoutine = StartCoroutine(PageEffectsRoutine());
    }


    IEnumerator PageEffectsRoutine()
    {
        while (true)
        {
            _glareEffect.anchoredPosition = new Vector2(0, -Screen.height - 1024);
            _glareEffect.DOAnchorPos(new Vector2(0, Screen.height + 1024), 3f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(5f);
        }
    }
}