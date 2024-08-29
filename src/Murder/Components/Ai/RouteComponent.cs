using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Serialization;

namespace Murder.Components
{
    [DoNotPersistOnSave(exceptIfComponentIsPresent: typeof(PersistPathfindComponent))]
    [Requires(typeof(PathfindComponent))]
    public readonly struct RouteComponent : IComponent
    {
        /// <summary>
        /// Nodes path that the agent will make.
        /// </summary>
        public readonly ComplexDictionary<Point, Point> Nodes;

        /// <summary>
        /// Initial position cell.
        /// </summary>
        public readonly Point Initial;

        /// <summary>
        /// Goal position cell.
        /// </summary>
        public readonly Point Target;

        public RouteComponent(ComplexDictionary<Point, Point> route, Point initial, Point target) =>
            (Nodes, Initial, Target) = (route, initial, target);
    }
}