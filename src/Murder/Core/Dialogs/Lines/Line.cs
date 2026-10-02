using Murder.Assets;
using Murder.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Core.Dialogs
{
    public enum LineFlags
    {
        None = 0,
        DoNotAllowSkip = 0b1
    }

    public readonly struct Line
    {
        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid? Speaker = null;

        public readonly string? Portrait = null;

        /// <summary>
        /// Optional sound event that will be fired with this line.
        /// </summary>
        public readonly string? Event = null;

        /// <summary>
        /// If the caption has a text, this will be the information.
        /// </summary>
        public readonly LocalizedString? Text = null;

        /// <summary>
        /// Delay in seconds.
        /// </summary>
        public readonly float? Delay = null;

        /// <summary>
        /// Optional action before the line is executed.
        /// </summary>
        [Default("\uf70c")]
        [Tooltip("Before line is fired...")]
        public readonly DialogAction? ActBeforeWith { get; init; } = null;

        [Default("\uf0ad")]
        [Tooltip("Line properties...")]
        public readonly LineFlags? Flags { get; init; } = null;

        public Line() { }

        public Line(Guid? speaker) => Speaker = speaker;

        /// <summary>
        /// Create a line with a text without any speaker.
        /// </summary>
        public Line(LocalizedString text) => Text = text;

        /// <summary>
        /// Create a line with a text. That won't be used as a timer.
        /// </summary>
        public Line(Guid speaker, LocalizedString text) => (Speaker, Text) = (speaker, text);

        /// <summary>
        /// Create a line with a delay. That won't be used as a text.
        /// </summary>
        public Line(Guid? speaker, float delay) => (Speaker, Delay) = (speaker, delay);

        public Line(Guid? speaker, string? portrait, LocalizedString? text, float? delay, string? @event) : this(speaker) =>
            (Portrait, Text, Delay, Event) = (portrait, text, delay, @event);

        public Line WithText(LocalizedString text) => new(Speaker, Portrait, text, Delay, Event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithDelay(float delay) => new(Speaker, Portrait, Text, delay, Event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithSpeaker(Guid speaker) => new(speaker, Portrait, Text, Delay, Event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithSpeakerAndPortrait(Guid speaker, string? portrait) => new(speaker, portrait, Text, Delay, Event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithPortrait(string? portrait) => new(Speaker, portrait, Text, Delay, Event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithEvent(string? @event) => new(Speaker, Portrait, Text, Delay, @event) { ActBeforeWith = ActBeforeWith, Flags = Flags };

        public Line WithFlags(LineFlags? flags) => this with { Flags = flags };

        [MemberNotNullWhen(true, nameof(Text))]
        public bool IsText => Text is not null;
    }
}