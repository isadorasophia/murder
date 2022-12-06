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

        public readonly bool DestroyAfterFinished;

        /// <summary>
        /// Fades the screen using the FadeScreenSystem
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="startedTime">Unscaled time  when this fade started</param>
        /// <param name="duration"></param>
        /// <param name="color"></param>
        /// <param name="destroyAfterFinished"></param>
        public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color, bool destroyAfterFinished = false) => 
            (Fade, StartedTime, Duration, Color, DestroyAfterFinished) = (fade, startedTime, duration, color, destroyAfterFinished);
    }
}
