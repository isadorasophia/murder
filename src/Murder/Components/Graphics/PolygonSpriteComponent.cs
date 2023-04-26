using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct PolygonSpriteComponent : IComponent
{
    public readonly ImmutableArray<IShape> Shapes = ImmutableArray<IShape>.Empty;
    public readonly Color Color;
    public PolygonSpriteComponent()
    {
    }
}