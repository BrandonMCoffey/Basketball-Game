using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private PlayerCardCatalog _cardPrefab;
    [SerializeField] private TMP_Text _cardsActiveCount;
    
    private List<PlayerCardCatalog> _cards;

    private void Start()
    {
        InitializeCards();
    }

    public void StartGame() => GameManager.Instance.LoadGameScene();

    private void InitializeCards()
    {
        var ownedPlayers = GameManager.Instance.OwnedPlayers;
        _cards = new List<PlayerCardCatalog>(ownedPlayers.Count);
        for (int i = 0; i < ownedPlayers.Count; i++)
        {
            var card = Instantiate(_cardPrefab, _grid.transform);
            if (card != null)
            {
                card.SetData(ownedPlayers[i]);
                card.SetIndex(i);
                //card.InitCardActive(instance.Players[i].PlayerEnabled);
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