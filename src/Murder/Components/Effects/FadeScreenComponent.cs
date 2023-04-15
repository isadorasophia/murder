using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components
{
    public enum FadeType
    {
        In,
        Out,
        Flash
    }

    [DoNotPersistEntityOnSave]
    public readonly struct FadeScreenComponent : IComponent
    {
        public readonly float StartedTime;

        public readonly float Duration;

        public readonly FadeType Fade;

        public readonly Color Color;
        
        public readonly string CustomTexture;

        public readonly bool DestroyAfterFinished;

        /// <summary>
        /// Fades the screen using the FadeScreenSystem
        /// </summary>
        public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color, bool destroyAfterFinished = false, string customTexture = "")
        {
            StartedTime = startedTime;
            Duration = duration;
            Fade = fade;
            Color = color;
            CustomTexture = customTexture;
            DestroyAfterFinished = destroyAfterFinished;
        }
    }
}
