public class UIShopState : UIStateBase
{
    public UIShopState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter() {
        _canvasController.ShopController.gameObject.SetActive(true);
        _canvasController.ShopController.AnimateOnScreen();
    }

    public override void OnExit() {
        _canvasController.ShopController.AnimateOffScreen(() => {
            _canvasController.ShopController.gameObject.SetActive(false);
        }); 
    }

}
