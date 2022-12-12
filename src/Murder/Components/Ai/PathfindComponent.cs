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

        public PathfindComponent(in Vector2 target, PathfindAlgorithmKind algorithm) =>
            (Target, Algorithm) = (target, algorithm);
    }
}
