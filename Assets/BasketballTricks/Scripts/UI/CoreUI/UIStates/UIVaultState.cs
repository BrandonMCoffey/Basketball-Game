using System;
using SaiUtils.StateMachine;
using UnityEngine;
using DG.Tweening;

public class UIVaultState : UIStateBase
{
    public UIVaultState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.menusContainer.DOAnchorPos(
            _canvasController.GetPosition(Menus.Vault),
            _canvasController.transitionDuration).SetEase(_canvasController.transitionEase);
    }
}
