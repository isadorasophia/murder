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

        /// <summary>
        /// Fades the screen using the FadeScreenSystem
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="startedTime">Unscaled time  when this fade started</param>
        /// <param name="duration"></param>
        /// <param name="color"></param>
        public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color) => 
            (Fade, StartedTime, Duration, Color) = (fade, startedTime, duration, color);
    }
}
