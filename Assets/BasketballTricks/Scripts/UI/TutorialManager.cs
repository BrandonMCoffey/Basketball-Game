using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayerCardData _startingPlayerCard;
    [SerializeField] private PlayerPosition _startingPlayerPos;
    [SerializeField] private PlayerActionData _spiderDribble;
    [SerializeField] private PlayerActionData _threePointer;
    [SerializeField] private PlayerActionData _chestPass;

    [SerializeField] private PlayerCardData _secondPlayerCard;
    [SerializeField] private PlayerPosition _secondPlayerPos;

    [Header("References")]
    [SerializeField] private ActionDeckManager _actionDeckManager;
    [SerializeField] private PlayerManager _playerManager;

    private Player _startingPlayer;
    private GameCard _startingGameCard;
    private Player _secondPlayer;
    private GameCard _secondGameCard;
    private int _hand = -1;

    public void Start()
    {
        _actionDeckManager.BeforeDrawingNextHand += NextHand;
        _hand = -1;
        NextHand();
    }

    private void NextHand()
    {
        List<GameAction> deck = new List<GameAction>();
        _hand++;
        switch (_hand)
        {
            case 0:
                _startingGameCard = GameManager.Instance.GetCard(_startingPlayerCard);
                foreach (var player in _playerManager.Players)
                {
                    if (player.Position == _startingPlayerPos)
                    {
                        _startingPlayer = player;
                        player.Place(_startingGameCard);
                    }
                    else
                    {
                        player.gameObject.SetActive(false);
                    }
                }
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                break;
            case 1:
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_threePointer)));
                break;
            case 2:
                _secondGameCard = GameManager.Instance.GetCard(_secondPlayerCard);
                foreach (var player in _playerManager.Players)
                {
                    if (player.Position == _secondPlayerPos)
                    {
                        _secondPlayer = player;
                        player.Place(_secondGameCard);
                    }
                }
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                deck.Add(new GameAction(_secondPlayer, _startingGameCard.GetActionIndex(_threePointer)));
                deck.Add(new GameAction(_secondPlayer, _startingGameCard.GetActionIndex(_chestPass)));
                break;
        }
        Debug.Log($"Deck size: {deck.Count}");
        _actionDeckManager.InitTutorial(deck, _hand == 0);
    }
}
