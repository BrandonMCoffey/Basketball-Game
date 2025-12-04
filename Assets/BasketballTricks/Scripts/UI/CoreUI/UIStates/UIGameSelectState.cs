using DG.Tweening;

public class UIGameSelectState : UIStateBase
{
    public UIGameSelectState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.GameSelectController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.GameSelectController.AnimateOffScreen(() =>
        {
            _canvasController.GameSelectController.gameObject.SetActive(false);
        });
    }
}
