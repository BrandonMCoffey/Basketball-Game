using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("UI References")]
    [SerializeField] private Image _inputBlocker;
    [SerializeField] private TutorialPanelController _startingPanel;

    private Player _startingPlayer;
    private GameCard _startingGameCard;
    private Player _secondPlayer;
    private GameCard _secondGameCard;
    private int _hand = -1;

    bool _finished = false;

    public void Start()
    {
        _actionDeckManager.BeforeDrawingNextHand += NextHand;
        _hand = -1;
        NextHand();

        if (_startingPanel != null) _startingPanel.gameObject.SetActive(true);
    }

    private void NextHand()
    {
        List<GameAction> deck = new List<GameAction>();
        _hand++;
        switch (_hand)
        {
            case 0:
                _startingGameCard = GameManager.Instance.GetMatchingCard(_startingPlayerCard);
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
                _secondGameCard = GameManager.Instance.GetMatchingCard(_secondPlayerCard);
                foreach (var player in _playerManager.Players)
                {
                    if (player.Position == _secondPlayerPos)
                    {
                        player.gameObject.SetActive(true);
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

    public void BlockInput(bool block)
    {
        _inputBlocker.DOFade(block ? 0.8f : 0f, 0.5f).OnComplete(() =>
        {
            _inputBlocker.raycastTarget = block;
        });
    }

    public void TutorialFinished()
    {
        _finished = true;
    }

}
