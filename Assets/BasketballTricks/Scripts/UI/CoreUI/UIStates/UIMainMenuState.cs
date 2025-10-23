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
    }
}
