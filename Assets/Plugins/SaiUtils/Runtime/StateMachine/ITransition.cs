namespace SaiUtils.StateMachine
{
    public interface ITransition
    {
        IState TargetState { get; }
        IPredicate Condition { get; }
    }

   
}