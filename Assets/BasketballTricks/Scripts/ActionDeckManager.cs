using SaiUtils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDeckManager : MonoBehaviour
{
    [SerializeField] private ActionCard _cardPrefab;
    [SerializeField] private int _handSize = 5;
    [SerializeField] private Transform _handPosition;

    private List<ActionDeckCard> _actionDeck = new List<ActionDeckCard>();

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
        for (int i = 0; i < count; i++)
        {
            var actionCard = Instantiate(_cardPrefab, _handPosition, false);
            var pos = actionCard.transform.position;
            pos.x = _handPosition.position.x + (i - 2) * 300f;
            actionCard.transform.position = pos;
            actionCard.Init(_actionDeck[i].Player.CardData.GetAction(_actionDeck[i].ActionIndex), _actionDeck[i].Player.PositionColor);
        }
    }
}

public struct ActionDeckCard
{
    public Player Player;
    public int ActionIndex;
}