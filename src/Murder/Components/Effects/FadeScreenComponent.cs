using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public enum FadeType
    {
        In,
        Out
    }

    [Unique]
    [DoNotPersistEntityOnSave]
    internal readonly struct FadeScreenComponent : IComponent
    {
        public readonly float StartedTime;

        public readonly float Duration;

        public readonly FadeType Fade;

        public FadeScreenComponent(FadeType fade, float startedTime, float duration) => 
            (Fade, StartedTime, Duration) = (fade, startedTime, duration);
    }
}
