using Bang.Components;
using Murder.Attributes;
using Murder.Core.Ai;
using Murder.Core.Geometry;

namespace Murder.Components
{
    [DoNotPersistOnSave]
    [Requires(typeof(ITransformComponent))]
    public readonly struct PathfindComponent : IComponent
    {
        public readonly Vector2 Target;

        public readonly PathfindAlgorithmKind Algorithm;

        public readonly float MaxSpeed;

        public readonly float Accel;

        public PathfindComponent(in Vector2 target, PathfindAlgorithmKind algorithm, float maxSpeed, float accel) =>
            (Target, Algorithm, MaxSpeed, Accel) = (target, algorithm, maxSpeed, accel);
    }
}
