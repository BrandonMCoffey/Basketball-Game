using DG.Tweening;

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
