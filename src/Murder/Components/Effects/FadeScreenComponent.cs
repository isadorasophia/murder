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

    [Unique]
    [DoNotPersistEntityOnSave]
    public readonly struct FadeScreenComponent : IComponent
    {
        public readonly float StartedTime;

        public readonly float Duration;

        public readonly FadeType Fade;

        public readonly Color Color;

        public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color) => 
            (Fade, StartedTime, Duration, Color) = (fade, startedTime, duration, color);
    }
}
