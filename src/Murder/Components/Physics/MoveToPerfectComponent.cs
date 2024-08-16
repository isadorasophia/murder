using Bang.Components;
using Murder.Attributes;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components
{
    /// <summary>
    /// This is a move to component that is not tied to any agent and
    /// that matches perfectly the target.
    /// </summary>
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct MoveToPerfectComponent : IComponent
    {
        public readonly Vector2 Target;
        public readonly Vector2? StartPosition = null;

        public readonly float StartTime;
        public readonly float Duration;

        public readonly EaseKind EaseKind;

        public readonly bool AvoidActors { get; init; } = true;

        private MoveToPerfectComponent(in Vector2 target, in Vector2 startPosition,
            float startTime, float duration, EaseKind ease) =>
            (Target, StartPosition, StartTime, Duration, EaseKind) =
            (target, startPosition, startTime, duration, ease);

        public MoveToPerfectComponent(in Vector2 target, float duration, EaseKind ease)
        {
            Target = target;

            StartTime = Game.Now;
            Duration = duration;

            EaseKind = ease;
        }

        public MoveToPerfectComponent WithStartPosition(in Vector2 startPosition) =>
            new(Target, startPosition, StartTime, Duration, EaseKind);
    }
}