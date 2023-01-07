using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    [Requires(typeof(ColliderComponent))]
    // TODO: Isa
    //[Requires(typeof(ITransformComponent), typeof(ColliderComponent))]
    [CustomName(" Collider")]
    public readonly struct ColliderComponent : IComponent
    {
        /// <summary>
        /// Value of layer according to <see cref="CollisionLayersBase"/>.
        /// </summary>
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

        /// <summary>
        /// Set layer according to <see cref="CollisionLayersBase"/>.
        /// </summary>
        public ColliderComponent SetLayer(int layer) => new ColliderComponent(Shapes, layer, DebugColor);
    }
}
