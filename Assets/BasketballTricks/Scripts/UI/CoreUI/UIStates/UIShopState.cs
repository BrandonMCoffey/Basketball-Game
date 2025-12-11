using DG.Tweening;

public class UIShopState : UIStateBase
{
    public UIShopState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.ShopController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.ShopController.gameObject.SetActive(false);
    }

}
