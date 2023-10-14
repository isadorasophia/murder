using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is used for carve components within the map (that will not move a lot and 
    /// should be taken into account on pathfinding).
    /// This a tag used when caching information in <see cref="Core.Map"/>.
    /// </summary>
    [Requires(typeof(ITransformComponent))]
    public readonly struct CarveComponent : IComponent
    {
        public readonly bool BlockVision = false;

        public readonly bool Obstacle = true;

        /// <summary>
        /// Whether this carve component will add a path if there was previously a collision in its area.
        /// For example, a bridge over a river.
        /// </summary>
        [Tooltip("Whether this carve component will add a path if there was " +
            "previously a collision in its area. For example, a bridge over a river.")]
        public readonly bool ClearPath = false;

        /// <summary>
        /// Weight of the component, if not an obstacle.
        /// Ignored if <see cref="Obstacle"/> is true.
        /// </summary>
        public readonly int Weight = -1;

        public CarveComponent() { }

        public CarveComponent(bool blockVision, bool obstacle, bool clearPath, int weight) =>
            (BlockVision, Obstacle, ClearPath, Weight) = (blockVision, obstacle, clearPath, weight);

        public CarveComponent WithClearPath(bool clearPath) => new(BlockVision, Obstacle, clearPath, Weight);

        public static CarveComponent CarveClearPath => new(blockVision: false, obstacle: false, clearPath: true, weight: 1);
    }
}