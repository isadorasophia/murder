using Bang.Components;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// For now, this will only fade out aseprite components.
    /// </summary>
    public readonly struct FadeTransitionComponent : IComponent
    {
        [JsonIgnore]
        public readonly float StartTime;

        /// <summary>
        /// Fade duration in seconds.
        /// </summary>
        public readonly float Duration;

        public readonly float StartAlpha;

        public readonly float TargetAlpha;

        public readonly bool DestroyEntityOnEnd;

        public FadeTransitionComponent(float duration, float startAlpha, float targetAlpha) : this(duration, startAlpha, targetAlpha, destroyOnEnd: false) { }

        public FadeTransitionComponent(float duration, float startAlpha, float targetAlpha, bool destroyOnEnd)
        {
            StartTime = Game.NowUnscaled;

            StartAlpha = startAlpha;
            TargetAlpha = targetAlpha;
            Duration = duration;
            DestroyEntityOnEnd = destroyOnEnd;
        }
    }
}