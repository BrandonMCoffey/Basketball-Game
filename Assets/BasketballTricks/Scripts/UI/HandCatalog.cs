using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCatalog : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private PlayerCard _cardPrefab;

    private List<PlayerCard> _cards;

    private void Start()
    {
        InitializeCards(GameManager.Instance);
    }

    public void StartGame() => GameManager.Instance.StartGame();

    private void InitializeCards(GameManager instance)
    {
        _cards = new List<PlayerCard>(instance.Players.Count);
        for (int i = 0; i < instance.Players.Count; i++)
        {
            if (!instance.Players[i].PlayerEnabled) continue;
            var card = Instantiate(_cardPrefab, _grid.transform);
            if (card != null)
            {
                card.SetData(instance.Players[i]);
                card.SetIndex(i);
                card.transform.localScale = Vector3.zero;
                _cards.Add(card);
            }
        }
        StartCoroutine(ShowCardsRoutine());
    }

    private IEnumerator ShowCardsRoutine()
    {
        while (GameManager.InTransition) yield return null;
        foreach (var card in _cards)
        {
            card.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
    }
}