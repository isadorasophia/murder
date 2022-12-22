using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.NoneOf, typeof(MapComponent))]
    internal class MapInitializerSystem : IStartupSystem
    {
        public void Start(Context context)
        {
            Entity? mapEntity;

            int width = 0;
            int height = 0;

            // First, get the dimensions of the map.
            ImmutableArray<Entity> gridEntities = context.World.GetEntitiesWith(typeof(TileGridComponent));
            for (int i = 0; i < gridEntities.Length; ++i)
            {
                TileGrid grid = gridEntities[i].GetTileGrid().Grid;

                width = Math.Max(width, grid.Width + grid.Origin.X);
                height = Math.Max(height, grid.Height + grid.Origin.Y);
            }

            mapEntity = context.World.AddEntity();
            mapEntity.SetMap(width, height);
            
            Map map = mapEntity.GetMap().Map;

            for (int i = 0; i < gridEntities.Length; ++i)
            {
                TileGrid grid = gridEntities[i].GetTileGrid().Grid;
                TilesetAsset[] assets = gridEntities[i].GetTileset().Tilesets.ToAssetArray<TilesetAsset>();

                InitializeMap(map, grid, assets);
            }
        }

        private void InitializeMap(Map map, TileGrid grid, TilesetAsset[] assets)
        {
            for (int y = grid.Origin.Y; y < grid.Height + grid.Origin.Y && y < map.Height; y++)
            {
                for (int x = grid.Origin.X; x < grid.Width + grid.Origin.X && x < map.Width; x++)
                {
                    // For each tile, we will check whether it is a solid or not.
                    // If so, we will check if the given grid has the flag set for that tile.
                    for (int i = 0; i < assets.Length; i++)
                    {
                        int mask = i.ToMask();
                        if (assets[i].IsSolid && grid.HasFlagAtGridPosition(x, y, mask))
                        {
                            map.SetOccupiedAsStatic(x, y);
                        }
                    }
                }
            }
        }
    }
}
