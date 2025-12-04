using DG.Tweening;

public class UIMainMenuState : UIStateBase
{
    public UIMainMenuState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.MainMenuController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.MainMenuController.AnimateOffScreen();
    }
    
}
