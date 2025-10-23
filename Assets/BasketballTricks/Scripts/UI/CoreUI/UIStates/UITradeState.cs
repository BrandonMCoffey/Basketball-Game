using System;
using DG.Tweening;
using SaiUtils.StateMachine;
using UnityEngine;

public class UITradeState : UIStateBase
{
    public UITradeState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.menusContainer.DOAnchorPos(
            _canvasController.GetPosition(Menus.Trade),
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.navBarContainer.Panel.DOAnchorPos(
            _canvasController.navBarContainer.OffScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.cardContainer.Panel.DOAnchorPos(_canvasController.cardContainer.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);

        _canvasController.tradeBoxesContainer.Panel.DOAnchorPos(_canvasController.tradeBoxesContainer.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
    }

    public override void OnExit()
    {
        _canvasController.navBarContainer.Panel.DOAnchorPos(
            _canvasController.navBarContainer.OnScreenPosition,
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
    }
}
