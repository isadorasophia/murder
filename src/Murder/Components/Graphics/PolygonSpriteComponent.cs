using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct PolygonSpriteComponent : IComponent
{
    public readonly ImmutableArray<IShape> Shapes = ImmutableArray<IShape>.Empty;
    [PaletteColor]
    public readonly Color Color = Color.White;
    public readonly TargetSpriteBatches Batch = TargetSpriteBatches.Gameplay;
    public readonly int SortOffset;

    public PolygonSpriteComponent()
    {
    }
}