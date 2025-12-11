using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCard : MonoBehaviour
{
    [SerializeField] private List<PlayerCardData> _potentialCards;
    [SerializeField] private PlayerCard _buyCard;
    [SerializeField] private LongButtonController _buyButton;

    private int _cost;

    private void Start()
    {
        _buyCard.SetData(new GameCard(_potentialCards[Random.Range(0, _potentialCards.Count - 1)]));
        _cost = Random.Range(3, 5) * 100;
        _buyButton.SetText($"Buy Card\n${_cost}");
    }

    public void PurchaseCard()
    {
        if (GameManager.Instance.AttemptSpendMoney(_cost))
        {
            GameManager.Instance.AddCardToOwned(_buyCard.Card.CardDataSO);
        }
    }
}
