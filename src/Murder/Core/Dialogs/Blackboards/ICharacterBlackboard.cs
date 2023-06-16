namespace Murder.Core.Dialogs
{
    /// <summary>
    /// This works similarly as a <see cref="IBlackboard"/>, except that each situation
    /// on the game has its own table.
    /// </summary>
    public abstract class ICharacterBlackboard : IBlackboard
    {
        public BlackboardKind Kind => BlackboardKind.Character;
    }
}
