using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct ChoiceLine
    {
        /// <summary>
        /// Dialog title.
        /// </summary>
        public readonly string Title = string.Empty;

        /// <summary>
        /// Choices available to the player to pick.
        /// </summary>
        public readonly ImmutableArray<string> Choices = ImmutableArray<string>.Empty;

        public ChoiceLine(string title, ImmutableArray<string> choices) =>
            (Title, Choices) = (title, choices);
    }
}
