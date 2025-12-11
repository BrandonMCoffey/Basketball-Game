using CoffeyUtils.Sound;

public class UIMainMenuState : UIStateBase
{
    public UIMainMenuState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        SoundManager.PlaySfx(SFXEventsEnum.UIMenuSwish);
        _canvasController.MainMenuController.gameObject.SetActive(true);
        _canvasController.MainMenuController.Enable();
    }

    public override void OnExit()
    {
        _canvasController.MainMenuController.AnimateOffScreen();
    }
    
}
