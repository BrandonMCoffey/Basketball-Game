using System.Collections.Generic;
using CoffeyUtils.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayerCardData _startingPlayerCard;
    [SerializeField] private PlayerPosition _startingPlayerPos;
    [SerializeField] private PlayerActionData _spiderDribble;
    [SerializeField] private PlayerActionData _threePointer;
    [SerializeField] private PlayerActionData _chestPass;
    [SerializeField] private PlayerActionData _alleyOop;
    [SerializeField] private PlayerActionData _dunk;

    [SerializeField] private PlayerCardData _secondPlayerCard;
    [SerializeField] private PlayerPosition _secondPlayerPos;

    [Header("References")]
    [SerializeField] private ActionDeckManager _actionDeckManager;
    [SerializeField] private PlayerManager _playerManager;

    [Header("UI References")]
    [SerializeField] private Image _inputBlocker;
    [SerializeField] private GameObject _startingPanel;
    [SerializeField] private GameObject _panelAtHand2;
    [SerializeField] private GameObject _panelAtHand3;
    [SerializeField] private GameObject _panelAtHand4;
    [SerializeField] CostDisplay _costDisplay;

    private Player _startingPlayer;
    private GameCard _startingGameCard;
    private Player _secondPlayer;
    private GameCard _secondGameCard;
    private int _hand = -1;

    bool _finished = false;

    private void OnEnable() {
        ActionDeckManager.OnSequenceEnd += SequenceEnd;
    }

    private void OnDisable() {
        ActionDeckManager.OnSequenceEnd -= SequenceEnd;
    }

    public void Start()
    {
        _actionDeckManager.BeforeDrawingNextHand += NextHand;
        _hand = 0;

        if (_startingPanel != null) _startingPanel.SetActive(true);

        SoundManager.PlayMusicNow(MusicTracksEnum.Tutorial);
    }

    public void NextHand()
    {
        List<GameAction> deck = new List<GameAction>();
        _hand++;
        switch (_hand)
        {
            case 1:
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
            case 2:
                if (_panelAtHand2 != null) _panelAtHand2.SetActive(true);
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_threePointer)));
                break;
            case 3:
                if (_panelAtHand3 != null) _panelAtHand3.SetActive(true);
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
                deck.Add(new GameAction(_secondPlayer, _secondGameCard.GetActionIndex(_chestPass)));
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_threePointer)));
                break;
            case 4:
                if (_panelAtHand4 != null) _panelAtHand4.SetActive(true);
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_alleyOop)));
                deck.Add(new GameAction(_startingPlayer, _startingGameCard.GetActionIndex(_spiderDribble)));
                deck.Add(new GameAction(_secondPlayer, _secondGameCard.GetActionIndex(_chestPass)));
                deck.Add(new GameAction(_secondPlayer, _secondGameCard.GetActionIndex(_dunk)));
                break;
        }
        Debug.Log($"Deck size: {deck.Count}");
        _actionDeckManager.InitTutorial(deck, _hand <= 1);
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

    private void SequenceEnd()
    {
        Invoke(nameof(Fin), 1f);
    }

    void Fin()
    {
        if (_finished)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }

    public void IncreaseEnergy()
    {
        _costDisplay.UpdateCostDisplay(0, 3);
    }
}