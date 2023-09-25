using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [TileEditor]
    [Filter(typeof(TileGridComponent))]
    [Watch(typeof(TileGridComponent))]
    public class GridCacheRenderSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            OnTileGridModified(world);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
        }

        public static void OnTileGridModified(World world)
        {
            ImmutableArray<Entity> gridEntities = world.GetEntitiesWith(typeof(TileGridComponent));
            foreach (Entity e in gridEntities)
            {
                TileGridComponent component = e.GetTileGrid();
                TileGrid grid = component.Grid;

                grid.UpdateCache(gridEntities);
            }
        }
    }
}
