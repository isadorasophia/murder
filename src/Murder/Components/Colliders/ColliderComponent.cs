using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

[CustomName(" Collider")]
public readonly struct ColliderComponent : IComponent, IEquatable<IComponent>
{
    /// <summary>
    /// Value of layer according to <see cref="CollisionLayersBase"/>.
    /// </summary>
    [CollisionLayer]
    public readonly int Layer = 0;

    public readonly ImmutableArray<IShape> Shapes { get; init; } = ImmutableArray<IShape>.Empty;

    public readonly Color DebugColor = Game.Profile.Theme.HighAccent;

    // Keep this so serialization is happy about uninitialized arrays.
    public ColliderComponent() => Shapes = ImmutableArray<IShape>.Empty;

    public ColliderComponent(IShape shape, int layer, Color color)
    {
        Shapes = new IShape[] { shape }.ToImmutableArray();
        DebugColor = color;
        Layer = layer;
    }
    public ColliderComponent(ImmutableArray<IShape> shapes, int layer, Color color)
    {
        Shapes = shapes;
        DebugColor = color;
        Layer = layer;
    }

    public bool Equals(IComponent? iOther)
    {
        if (iOther is not ColliderComponent other)
        {
            return false;
        }

        if (Layer != other.Layer || DebugColor != other.DebugColor)
        {
            return false;
        }

        if (Shapes.Length != other.Shapes.Length)
        {
            return false;
        }

        for (int i = 0; i < Shapes.Length; ++i)
        {
            if (!Shapes[i].Equals(other.Shapes[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Whether a layer from <see cref="CollisionLayersBase"/> is present.
    /// </summary>
    public bool HasLayer(int layer) => (Layer & layer) != 0;

    /// <summary>
    /// Set layer according to <see cref="CollisionLayersBase"/>.
    /// </summary>
    public ColliderComponent SetLayer(int layer) => new ColliderComponent(Shapes, layer, DebugColor);

    /// <summary>
    /// Set layer according to <see cref="CollisionLayersBase"/>.
    /// </summary>
    public ColliderComponent WithLayerFlag(int flag) => new ColliderComponent(Shapes, Layer | flag, DebugColor);

    /// <summary>
    /// Set layer according to <see cref="CollisionLayersBase"/>.
    /// </summary>
    public ColliderComponent WithoutLayerFlag(int flag) => new ColliderComponent(Shapes, Layer & ~flag, DebugColor);
}