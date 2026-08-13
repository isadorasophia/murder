using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(typeof(TileGridComponent))]
    public class MapInitializerSystem : IStartupSystem
    {
        public void Start(Context context)
        {
            Entity? mapEntity;

            Point origin = new(int.MaxValue, int.MaxValue);

            int width = 25;
            int height = 25;

            // First, get the dimensions of the map.
            ImmutableArray<Entity> gridEntities = context.Entities;
            for (int i = 0; i < gridEntities.Length; ++i)
            {
                TileGrid grid = gridEntities[i].GetTileGrid().Grid;

                if (grid.Origin.X < origin.X)
                {
                    origin = new(grid.Origin.X, origin.Y);
                }

                if (grid.Origin.Y < origin.Y)
                {
                    origin = new(origin.X, grid.Origin.Y);
                }

                width = Math.Max(width, grid.Width + grid.Origin.X);
                height = Math.Max(height, grid.Height + grid.Origin.Y);
            }

            mapEntity = context.World.AddEntity();
            mapEntity.SetMap(origin, width, height);

            if (context.World.TryGetUniqueTileset()?.Tilesets is not ImmutableArray<Guid> tilesets)
            {
                return;
            }

            TilesetAsset?[] assets = tilesets.ToTilesetArray();
            Map map = mapEntity.GetMap().Map;

            for (int i = 0; i < gridEntities.Length; ++i)
            {
                TileGrid grid = gridEntities[i].GetTileGrid().Grid;
                RoomComponent room = gridEntities[i].GetRoom();

                InitializeMap(map, grid, room, assets);
                InitializeRoom(gridEntities[i], grid, room);
            }

            InitializeEmptyTiles(map, gridEntities);
        }

        private void InitializeMap(Map map, TileGrid grid, RoomComponent room, TilesetAsset?[] assets)
        {
            List<int> orderedAssets = GridHelper.GetOrderedTilesetIndices(assets);

            for (int y = grid.Origin.Y; y < grid.Height + grid.Origin.Y && y < map.Height; y++)
            {
                for (int x = grid.Origin.X; x < grid.Width + grid.Origin.X && x < map.Width; x++)
                {
                    if (Game.Data.TryGetAsset<FloorAsset>(room.Floor) is FloorAsset floor)
                    {
                        ApplyPropertyOnTile(map, x, y, floor.Properties, floor.Guid, ysort: 0);
                    }

                    // For each tile, we will check whether it is a solid or not.
                    // If so, we will check if the given grid has the flag set for that tile.
                    foreach (int index in orderedAssets)
                    {
                        if (assets[index] is not TilesetAsset asset)
                        {
                            continue;
                        }

                        int mask = index.ToMask();
                        if (grid.HasFlagAtGridPosition(x, y, mask))
                        {
                            map.SetOccupiedAsStatic(x, y, asset.CollisionLayer);
                            ApplyPropertyOnTile(map, x, y, asset.Properties, asset.Guid, ysort: asset.YSortOffset);
                        }
                    }
                }
            }
        }

        protected virtual void InitializeRoom(Entity e, TileGrid grid, RoomComponent room) { }

        /// <summary>
        /// Initialize a tile in the map according to its properties.
        /// </summary>
        /// <param name="map">Map instance.</param>
        /// <param name="x">X coordinate in the grid.</param>
        /// <param name="y">Y coordinate in the grid.</param>
        /// <param name="properties">Tile properties.</param>
        /// <param name="guid">Guid of the floor or tileset asset.</param>
        /// <param name="ysort">If applicable, the ysort offset that will be rendered.</param>
        protected virtual void ApplyPropertyOnTile(Map map, int x, int y, ITileProperties? properties, Guid guid, int ysort) { }

        private void InitializeEmptyTiles(Map map, ImmutableArray<Entity> grids)
        {
            int i = 0;
            IntRectangle[] gridAreas = new IntRectangle[grids.Length];
            foreach (Entity g in grids)
            {
                TileGrid grid = g.GetTileGrid().Grid;
                gridAreas[i++] = new IntRectangle(x: grid.Origin.X, y: grid.Origin.Y, width: grid.Width, height: grid.Height);
            }

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    bool isInsideRoom = false;
                    foreach (IntRectangle a in gridAreas)
                    {
                        if (a.Contains(x, y))
                        {
                            isInsideRoom = true;
                            break;
                        }
                    }

                    if (!isInsideRoom)
                    {
                        map.SetOccupiedAsStatic(x, y, CollisionLayersBase.SOLID);
                    }
                }
            }
        }
    }
}