namespace Murder.Core.Dialogs
{
    /// <summary>
    /// This is a blackboard tracker with dialogue variables.
    /// This is used when defining the set of conditions which will play a given dialog.
    /// </summary>
    public interface IBlackboard
    {
        public virtual BlackboardKind Kind => BlackboardKind.Gameplay;
    }
}
