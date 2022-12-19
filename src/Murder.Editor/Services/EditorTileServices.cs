using Bang.Entities;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core;
using System.Collections.Immutable;
using Bang;
using Murder.Editor.Utilities;

namespace Murder.Editor.Services
{
    public static class EditorTileServices
    {
        public static string? FindTargetGroup(World world, EditorHook hook, Point position)
        {
            ImmutableArray<Entity> entities = world.GetEntitiesWith(typeof(TileGridComponent));
            foreach (Entity grid in entities)
            {
                IntRectangle bounds = grid.GetTileGrid().Rectangle * Grid.CellSize;
                if (bounds.Contains(position))
                {
                    return hook.TryGetGroupNameForEntity(grid.EntityId);
                }
            }

            return null;
        }
    }
}
