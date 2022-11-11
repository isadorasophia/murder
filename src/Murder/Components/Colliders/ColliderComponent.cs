using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using System.Collections.Immutable;

namespace Murder.Components
{
    [Requires(typeof(ITransformComponent), typeof(ColliderComponent))]
    public readonly struct ColliderComponent : IComponent
    {
        public readonly bool Solid = true;

        public readonly ImmutableArray<IShape> Shapes = ImmutableArray<IShape>.Empty;

        public readonly Color DebugColor = Color.Red;

        // Keep this so serialization is happy about uninitialized arrays.
        public ColliderComponent() => Shapes = ImmutableArray<IShape>.Empty;

        public ColliderComponent(ImmutableArray<IShape> shapes, bool solid, Color color)
        {
            Shapes = shapes;
            Solid = solid;
            DebugColor = color;
        }
    }
}
