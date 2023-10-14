namespace Murder.Core.Dialogs
{
    public enum MatchKind
    {
        /// <summary>
        /// This will pick in consecutive order, whatever matches first.
        /// </summary>
        Next = 1,

        /// <summary>
        /// This will pick random dialogs.
        /// </summary>
        Random = 2,

        /// <summary>
        /// This will pick the dialog with the highest score.
        /// This is when dialogs are listed with -/+.
        /// </summary>
        HighestScore = 3,

        /// <summary>
        /// All the blocks that are next are subjected to an "else" relationship.
        /// </summary>
        IfElse = 4,

        /// <summary>
        /// Choice dialogs (>) that the player can pick.
        /// </summary>
        Choice = 5
    }
}