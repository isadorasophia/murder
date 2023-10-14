namespace Murder.Core.Dialogs
{
    /// <summary>
    /// This tracks and listens to parameters relevant to music and sound.
    /// </summary>
    public abstract class ISoundBlackboard : IBlackboard
    {
        public BlackboardKind Kind => BlackboardKind.Sound;
    }
}