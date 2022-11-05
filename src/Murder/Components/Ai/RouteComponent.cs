using Bang.Components;
using System.Collections.Immutable;
using Murder.Core.Geometry;
using Murder.Attributes;

namespace Murder.Components
{
    [DoNotPersistOnSave]
    [Requires(typeof(PathfindComponent))]
    public readonly struct RouteComponent : IComponent
    {
        /// <summary>
        /// Nodes path that the agent will make.
        /// </summary>
        public readonly ImmutableDictionary<Point, Point> Nodes;

        /// <summary>
        /// Initial position cell.
        /// </summary>
        public readonly Point Initial;

        /// <summary>
        /// Goal position cell.
        /// </summary>
        public readonly Point Target;

        public RouteComponent(ImmutableDictionary<Point, Point> route, Point initial, Point target) =>
            (Nodes, Initial, Target) = (route, initial, target);
    }
}
