using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a move to component that is not tied to any agent and
    /// that matches perfectly the target.
    /// </summary>
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct MoveToPerfectComponent : IComponent
    {
        public readonly Vector2 Target;

        public readonly float Start;
        public readonly float Duration;
        
        public readonly EaseKind EaseKind;

        public MoveToPerfectComponent(in Vector2 target, float duration, EaseKind ease)
        {
            Start = Game.Now;

            Target = target;
            Duration = duration;
            EaseKind = ease;
        }
    }
}
