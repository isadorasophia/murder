using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Components;

public readonly struct CellProperties
{
    public readonly Point Point { get; init; }

    public readonly int Weight { get; init; }

    [CollisionLayer]
    public readonly int CollisionMask { get; init; } = CollisionLayersBase.NONE;

    public CellProperties(Point coordinates) : this(coordinates, weight: 0, CollisionLayersBase.NONE) { }

    public CellProperties(Point coordinates, int weight, int collisionMask) =>
        (Point, Weight, CollisionMask) = (coordinates, weight, collisionMask);
}

[Unique]
public readonly struct PathfindGridComponent : IComponent
{
    /// <summary>
    /// Unique collection of cell properties. "How do we keep this unique", you ask?
    /// This is responsability of the editor editing this. This cannot be a dictionary (I would love to!)
    /// because it will break the deterministic behavior of the world.
    /// </summary>
    public readonly ImmutableArray<CellProperties> Cells = [];

    public PathfindGridComponent() { }

    [JsonConstructor]
    public PathfindGridComponent(ImmutableArray<CellProperties> cells) => Cells = cells;
}
