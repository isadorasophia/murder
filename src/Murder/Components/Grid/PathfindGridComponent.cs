using Bang.Components;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct CellProperties
{
    public readonly Point Coordinates;

    public readonly int Weight;

    [CollisionLayer]
    public readonly int CollisionMask = CollisionLayersBase.NONE;

    public CellProperties(Point coordinates, int weight, int collisionMask) =>
        (Coordinates, Weight, CollisionMask) = (coordinates, weight, collisionMask);
}

public readonly struct PathfindGridComponent : IComponent
{
    /// <summary>
    /// Unique collection of cell properties. "How do we keep this unique", you ask?
    /// This is responsability of the editor editing this. This cannot be a dictionary (I would love to!)
    /// because it will break the deterministic behavior of the world.
    /// </summary>
    public readonly ImmutableArray<CellProperties> Cells = [];

    public PathfindGridComponent() { }
}
