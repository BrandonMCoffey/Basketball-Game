using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITradeState : UIStateBase
{
    public UITradeState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.TradeController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.TradeController.gameObject.SetActive(false);
    }
}
