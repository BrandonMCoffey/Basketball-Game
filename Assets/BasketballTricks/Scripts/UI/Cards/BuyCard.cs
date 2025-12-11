using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCard : MonoBehaviour
{
    [SerializeField] private List<PlayerCardData> _potentialCards;
    [SerializeField] private PlayerCard _buyCard;
    [SerializeField] private LongButtonController _buyButton;
    [SerializeField] private int _minCost = 3;
    [SerializeField] private int _maxCost = 5;

    private int _cost;

    private void Awake()
    {
        _buyCard.Init(transform, transform, false, Vector3.zero);
    }

    private void OnEnable()
    {
        _buyCard.RefreshTransform();
        _buyCard.SetData(new GameCard(_potentialCards[Random.Range(0, _potentialCards.Count - 1)]));
        _cost = Random.Range(_minCost, _maxCost) * 100;
        _buyButton.SetText($"Buy Card\n${_cost}");
        _buyButton.Interactable = true;
    }

    public void PurchaseCard()
    {
        if (GameManager.Instance.AttemptSpendMoney(_cost))
        {
            GameManager.Instance.AddCardToOwned(_buyCard.Card.CardDataSO);
            _buyButton.Interactable = false;
            var rectT = _buyCard.GetComponent<RectTransform>();
            rectT.DOAnchorPos(rectT.anchoredPosition + new Vector2(0, 200), 0.5f).SetEase(Ease.OutBack);
            rectT.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        }
    }
}
