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
    [Filter(typeof(MapDimensionsComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(MapComponent))]
    internal class MapInitializerSystem : IStartupSystem
    {
        public ValueTask Start(Context context)
        {
            Entity? mapEntity;

            int width;
            int height;

            if (context.HasAnyEntity)
            {
                mapEntity = context.Entity;

                MapDimensionsComponent dimensionsComponent = mapEntity.GetMapDimensions();
                width = dimensionsComponent.Width;
                height = dimensionsComponent.Height;
            }
            else
            {
                GameLogger.Warning("No entity of MapDimensionsComponent found, using default size.");

                mapEntity = context.World.AddEntity();
                width = height = 256;
            }

            mapEntity.SetMap(width, height);
            Map map = mapEntity.GetMap().Map;

            ImmutableArray<Entity> grids = context.World.GetEntitiesWith(typeof(TileGridComponent));
            foreach (Entity e in grids)
            {
                TileGrid grid = e.GetTileGrid().Grid;
                TilesetAsset[] assets = e.GetTileset().Tilesets.ToAssetArray<TilesetAsset>();
                
                InitializeMap(map, grid, assets);
            }

            return default;
        }

        private void InitializeMap(Map map, TileGrid grid, TilesetAsset[] assets)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    // For each tile, we will check whether it is a solid or not.
                    // If so, we will check if the given grid has the flag set for that tile.
                    for (int i = 0; i < assets.Length; i++)
                    {
                        int mask = i.ToMask();
                        if (assets[i].IsSolid && grid.HasFlagAtGridPosition(x, y, mask))
                        {
                            map.SetOccupiedAsStatic(x + grid.Origin.X, y + grid.Origin.Y);
                        }
                    }
                }
            }
        }
    }
}
