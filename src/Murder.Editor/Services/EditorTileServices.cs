using Bang;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Services
{
    public static class EditorTileServices
    {
        public static string? FindTargetGroup(World world, EditorHook hook, Point position)
        {
            if (hook.FocusGroup is not null)
            {
                return hook.FocusGroup;
            }

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