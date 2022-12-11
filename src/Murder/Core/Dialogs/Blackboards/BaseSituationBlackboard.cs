namespace Murder.Core.Dialogs
{
    /// <summary>
    /// Built-in capabilities for each speaker blackboard.
    /// </summary>
    [Blackboard(Name)]
    public class BaseSituationBlackboard : ISituationBlackboard
    {
        public const string Name = "BaseSpeaker";

        /// <summary>
        /// Total of times that it has been interacted to.
        /// </summary>
        public int TotalInteractions = 0;
    }
}
