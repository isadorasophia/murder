using Bang.Components;
using Murder.Attributes;
using Murder.Core.Ai;
using System.Numerics;

namespace Murder.Components
{
    [DoNotPersistOnSave(exceptIfComponentIsPresent: typeof(PersistPathfindComponent))]
    [Requires(typeof(PositionComponent))]
    public readonly struct PathfindComponent : IComponent
    {
        public readonly Vector2 Target;

        public readonly PathfindAlgorithmKind Algorithm;

        public PathfindComponent(in Vector2 target, PathfindAlgorithmKind algorithm) =>
            (Target, Algorithm) = (target, algorithm);
    }
}