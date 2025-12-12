public class UIVaultState : UIStateBase
{
    public UIVaultState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.VaultController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.VaultController.gameObject.SetActive(false);
    }
    
}
