using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    [Requires(typeof(ITransformComponent), typeof(ColliderComponent))]
    public readonly struct ColliderComponent : IComponent
    {
        [CollisionLayer]
        public readonly int Layer = 0;

        public readonly ImmutableArray<IShape> Shapes = ImmutableArray<IShape>.Empty;

        public readonly Color DebugColor = Color.Red;

        // Keep this so serialization is happy about uninitialized arrays.
        public ColliderComponent() => Shapes = ImmutableArray<IShape>.Empty;

        public ColliderComponent(ImmutableArray<IShape> shapes, int layer, Color color)
        {
            Shapes = shapes;
            DebugColor = color;
            Layer = layer;
        }
    }
}
