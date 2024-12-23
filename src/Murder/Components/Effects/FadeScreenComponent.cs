using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components
{
    public enum FadeType
    {
        In,
        Out,
        OutBuffer,
        Flash
    }

    [DoNotPersistOnSave]
    public readonly struct FadeScreenComponent : IComponent
    {
        public readonly float StartedTime { get; init; }

        public readonly float Duration;

        public readonly FadeType Fade;

        public readonly Color Color;

        public readonly string CustomTexture;

        public readonly float Sorting = 0;

        public readonly int BufferFrames { get; init; } = 0;

        public readonly int? TargetBatch { get; init; } = null;

        /// <summary>
        /// Fades the screen using the FadeScreenSystem. Duration will be a minimum of 0.1
        /// </summary>
        public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color, string customTexture = "", float sorting = 0, int bufferFrames = 0)
        {
            StartedTime = startedTime;
            Duration = MathF.Max(duration - 0.1f, 0.1f);
            Fade = fade;
            Color = color;
            CustomTexture = customTexture;
            Sorting = sorting;
            BufferFrames = bufferFrames;
        }
    }
}