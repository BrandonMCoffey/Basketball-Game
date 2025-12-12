public class UILeaderboardState : UIStateBase
{
    public UILeaderboardState(UICanvasController canvasController) : base(canvasController) { }

    public override void OnEnter()
    {
        _canvasController.LeaderboardController.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        _canvasController.LeaderboardController.gameObject.SetActive(false);
    }
}
