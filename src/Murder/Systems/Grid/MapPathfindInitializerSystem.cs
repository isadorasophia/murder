using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(PathfindGridComponent))]
public class MapPathfindInitializerSystem : IStartupSystem
{
    public void Start(Context context)
    {
        if (!context.HasAnyEntity)
        {
            return;
        }

        if (context.World.TryGetUniqueMap() is not MapComponent map)
        {
            return;
        }

        ImmutableArray<CellProperties> cells = context.Entity.GetPathfindGrid().Cells;
        foreach (CellProperties cell in cells)
        {
            map.Map.SetOccupied(cell.Point, cell.CollisionMask, cell.Weight);
        }
    }
}