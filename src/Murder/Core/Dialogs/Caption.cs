using Murder.Assets;
using Murder.Attributes;

namespace Murder.Core.Dialogs
{
    public readonly struct Line
    {
        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid Speaker = Guid.Empty;

        public readonly string Portrait = string.Empty;

        /// <summary>
        /// If the caption has a text, this will be the information.
        /// </summary>
        public readonly string? Text = null;

        /// <summary>
        /// Delay in seconds.
        /// </summary>
        public readonly float? Delay = null;

        public Line() { }

        public Line(Guid speaker) => Speaker = speaker;

        /// <summary>
        /// Create a line with a text. That won't be used as a timer.
        /// </summary>
        public Line(Guid speaker, string text) => (Speaker, Text) = (speaker, text);

        /// <summary>
        /// Create a line with a delay. That won't be used as a text.
        /// </summary>
        public Line(Guid speaker, float delay) => (Speaker, Delay) = (speaker, delay);

        public Line(Guid speaker, string portrait, string? text, float? delay) : this(speaker) =>
            (Portrait, Text, Delay) = (portrait, text, delay);

        public Line WithText(string text) => new(Speaker, Portrait, text, Delay);

        public Line WithDelay(float delay) => new(Speaker, Portrait, Text, delay);

        public Line WithSpeaker(Guid speaker) => new(speaker, Portrait, Text, Delay);

        public Line WithPortrait(string portrait) => new(Speaker, portrait, Text, Delay);

        public bool IsText => Text is not null;
    }
}
