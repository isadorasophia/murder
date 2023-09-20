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

    [SpriteBatchReference]
    public readonly int Batch = Batches2D.GameplayBatchId;
    
    public readonly int SortOffset;

    public PolygonSpriteComponent()
    {
    }
}