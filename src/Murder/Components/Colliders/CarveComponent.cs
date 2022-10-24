using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// This is used for carve components within the map (that will not move a lot and 
    /// should be taken into account on pathfinding).
    /// This a tag used when caching information in <see cref="Core.Map"/>.
    /// </summary>
    [Requires(typeof(PositionComponent))]
    public readonly struct CarveComponent : IComponent
    {
        public readonly bool BlockVision = false;

        public readonly bool Obstacle = true;

        /// <summary>
        /// Weight of the component, if not an obstacle.
        /// Ignored if <see cref="Obstacle"/> is true.
        /// </summary>
        public readonly int Weight = -1;

        public CarveComponent() { }
    }
}