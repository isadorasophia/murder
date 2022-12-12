using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;

namespace Murder.Components
{
    [DoNotPersistOnSave]
    public readonly struct MoveToComponent : IComponent
    {
        public readonly Vector2 Target;
        public readonly float MinDistance = 4;
        public readonly float SlowDownDistance = 12;

        public MoveToComponent(in Vector2 target)
        {
            Target = target;
        }
    }
}
