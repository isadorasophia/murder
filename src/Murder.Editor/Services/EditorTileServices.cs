using Bang;
using Murder.Core.Geometry;
using Murder.Editor.Utilities;

namespace Murder.Editor.Services
{
    public static class EditorTileServices
    {
        public static string? FindTargetGroup(World _, EditorHook hook, Point __)
        {
            if (hook.FocusGroup is not null)
            {
                return hook.FocusGroup;
            }

            // TODO: We are no longer supporting adding entities tied to a specific room.
            // Turns out this is pretty useless. I'll keep it here if we ever change our minds.
            //ImmutableArray<Entity> entities = world.GetEntitiesWith(typeof(TileGridComponent));
            //foreach (Entity grid in entities)
            //{
            //    IntRectangle bounds = grid.GetTileGrid().Rectangle * Grid.CellSize;
            //    if (bounds.Contains(position))
            //    {
            //        return hook.TryGetGroupNameForEntity(grid.EntityId);
            //    }
            //}

            return null;
        }
    }
}