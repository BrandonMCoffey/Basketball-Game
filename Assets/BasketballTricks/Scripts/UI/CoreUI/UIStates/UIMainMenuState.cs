using System;
using SaiUtils.StateMachine;
using UnityEngine;
using DG.Tweening;

public class UIMainMenuState : UIStateBase
{
    public UIMainMenuState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.menusContainer.DOAnchorPos(
            _canvasController.GetPosition(Menus.MainMenu),
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.playerData.Panel.DOAnchorPos(_canvasController.playerData.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.campaignButton.Panel.DOAnchorPos(_canvasController.campaignButton.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.tournamentButton.Panel.DOAnchorPos(_canvasController.tournamentButton.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.tradeButton.Panel.DOAnchorPos(_canvasController.tradeButton.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
    }

    public override void OnExit()
    {
        _canvasController.playerData.Panel.DOAnchorPos(_canvasController.playerData.OffScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase).OnComplete(() =>
            {
                _canvasController.campaignButton.Panel.DOAnchorPos(_canvasController.campaignButton.OffScreenPosition,
                    _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

                _canvasController.tournamentButton.Panel.DOAnchorPos(_canvasController.tournamentButton.OffScreenPosition,
                    _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

                _canvasController.tradeButton.Panel.DOAnchorPos(_canvasController.tradeButton.OffScreenPosition,
                    _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
            });

        
    }
}
