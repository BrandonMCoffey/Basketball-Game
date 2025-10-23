using SaiUtils.StateMachine;

public class UIStateBase : IState
{
	protected UICanvasController _canvasController;

	public UIStateBase(UICanvasController canvasController)
	{
		_canvasController = canvasController;
	}

	public virtual void OnEnter() { }
	public virtual void OnExit() { }
	public virtual void FixedUpdate() { }
	public virtual void Update() { }
}