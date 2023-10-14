using Murder.Assets;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct ChoiceLine
    {
        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid? Speaker = null;

        public readonly string? Portrait = null;

        /// <summary>
        /// Dialog title.
        /// </summary>
        public readonly string Title = string.Empty;

        /// <summary>
        /// Choices available to the player to pick.
        /// </summary>
        public readonly ImmutableArray<string> Choices = ImmutableArray<string>.Empty;

        public ChoiceLine(Guid speaker, string? portrait, string title, ImmutableArray<string> choices) =>
            (Speaker, Portrait, Title, Choices) = (speaker, portrait, title, choices);
    }
}