using Bang.Components;
using Murder.Attributes;
using System.Numerics;

namespace Murder.Components
{
    [DoNotPersistOnSave]
    public readonly struct MoveToComponent : IComponent
    {
        public readonly Vector2 Target;
        public readonly float MinDistance = 2;
        public readonly float SlowDownDistance = 12;

        public MoveToComponent(in Vector2 target)
        {
            Target = target;
        }
        public MoveToComponent(in Vector2 target, float minDistance, float slowDownDistance)
        {
            Target = target;
            MinDistance = minDistance;
            SlowDownDistance = slowDownDistance;
        }
    }
}