using System;
using DG.Tweening;
using SaiUtils.StateMachine;
using UnityEngine;

public class UIStoreState : UIStateBase
{
    public UIStoreState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.menusContainer.DOAnchorPos(
            _canvasController.GetPosition(Menus.Store),
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
    }
}
