using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaiUtils.StateMachine;
using DG.Tweening;

public class UICanvasController : MonoBehaviour
{
    private StateMachine _stateMachine;
    
    [Header("Main Menu References")]
    [SerializeField] private RectTransform _menusContainer;
    [SerializeField] private RectTransform _navBarContainer;
    [SerializeField] private RectTransform _playerData;
    [SerializeField] private RectTransform _campaignButton;
    [SerializeField] private RectTransform _tournamentButton;
    [SerializeField] private RectTransform _tradeButton;

    [Header("Vault References")]
    [SerializeField] private RectTransform _cardContainer;
    [SerializeField] private RectTransform _tradeBoxesContainer;
    bool _toggle = false;

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _toggle = !_toggle;
            if (_toggle) { ToVaultTransition(); }
            else { ToMainMenuTransition(); }
        }
    }

    void ToVaultTransition()
    {
        _playerData.DOAnchorPosY(350, 0.25f).SetEase(Ease.OutCubic);
        _navBarContainer.DOAnchorPosY(-250, 0.25f).SetEase(Ease.OutCubic);
        _campaignButton.DOAnchorPosY(-900, 0.25f).SetEase(Ease.OutCubic);
        _tournamentButton.DOAnchorPosY(-1800, 0.25f).SetEase(Ease.OutCubic);
        _tradeButton.DOAnchorPosY(-2700, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            _menusContainer.DOAnchorPosX(1920, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                _campaignButton.DORotate(new Vector3(0, 0, -70), 0.25f).SetEase(Ease.OutCubic);
                _tournamentButton.DORotate(new Vector3(0, 0, -90), 0.25f).SetEase(Ease.OutCubic);
                _tradeButton.DORotate(new Vector3(0, 0, -110), 0.25f).SetEase(Ease.OutCubic);
                _cardContainer.DOAnchorPosY(-210, 0.25f).SetEase(Ease.OutCubic);
                _tradeBoxesContainer.DOAnchorPosY(-660, 0.25f).SetEase(Ease.OutCubic);
            });
        });
    }
    void ToMainMenuTransition()
    {
        _cardContainer.DOAnchorPosY(500, 0.25f).SetEase(Ease.OutCubic);
        _tradeBoxesContainer.DOAnchorPosY(-1500, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
        { 
            _menusContainer.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                _playerData.DOAnchorPosY(0, 0.25f).SetEase(Ease.OutCubic);
                _campaignButton.DOAnchorPosY(-170, 0.25f).SetEase(Ease.OutCubic);
                _campaignButton.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutCubic);
                _tournamentButton.DOAnchorPosY(-170, 0.35f).SetEase(Ease.OutCubic);
                _tournamentButton.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.OutCubic);
                _tradeButton.DOAnchorPosY(-170, 0.45f).SetEase(Ease.OutCubic);
                _tradeButton.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutCubic);
            });
            _navBarContainer.DOAnchorPosY(0, 0.25f).SetEase(Ease.OutCubic);
        });
    }
}
