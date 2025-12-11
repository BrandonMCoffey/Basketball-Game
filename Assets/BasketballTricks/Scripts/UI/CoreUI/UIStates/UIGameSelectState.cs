using CoffeyUtils.Sound;

public class UIGameSelectState : UIStateBase
{
    public UIGameSelectState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        SoundManager.PlaySfx(SFXEventsEnum.UIMenuSwish);
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
