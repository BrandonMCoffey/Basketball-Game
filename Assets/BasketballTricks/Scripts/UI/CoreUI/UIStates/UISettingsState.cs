public class UISettingsState : UIStateBase
{
    public UISettingsState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.SettingsController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.SettingsController.gameObject.SetActive(false);
    }
   
}
