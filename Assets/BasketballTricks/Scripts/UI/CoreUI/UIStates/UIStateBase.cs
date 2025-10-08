using System;
using SaiUtils.StateMachine;

public class UIStateBase : IState
{
	UICanvasController _canvasController;
	public UIStateBase(UICanvasController canvasController)
	{
		_canvasController = canvasController;
	}

	public void OnEnter() { }
	public void OnExit() { }
	public void FixedUpdate() { }
	public void Update() { }
}