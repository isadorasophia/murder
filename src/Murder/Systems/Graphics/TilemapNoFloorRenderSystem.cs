using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Graphics;


/// <summary>
/// Draws only the Tilemap and not the floor. Will still draw aditional tiles on floor tiles, like reflections.
/// </summary>
[Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TileGridComponent))]
public class TilemapNoFloorRenderSystem : IMurderRenderSystem
{
    TilesetAsset[]? _tilesetAssetsCache = null;

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUniqueTileset() is not TilesetComponent tilesetComponent)
        {
            // Skip drawing on empty.
            return;
        }

        if (_tilesetAssetsCache == null)
        {
            _tilesetAssetsCache = tilesetComponent.Tilesets.ToAssetArray<TilesetAsset>();
        }

        // Iterate over each room.
        foreach (Entity e in context.Entities)
        {
            if (tilesetComponent.Tilesets.IsEmpty ||
                e.TryGetRoom()?.Floor is not Guid floorGuid ||
                Game.Data.TryGetAsset<FloorAsset>(floorGuid) is not FloorAsset floorAsset)
            {
                // Nothing to be drawn.
                continue;
            }

            TileGridComponent gridComponent = e.GetTileGrid();
            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(gridComponent.Rectangle);
            TileGrid grid = gridComponent.Grid;

            for (int i = 0; i < _tilesetAssetsCache.Length; ++i)
            {
                var asset = _tilesetAssetsCache[i];
                if (asset == null || (asset.TargetBatch == ((int)Batches2D.FloorBatchId) && asset.AdditionalTiles.Length == 0))
                    continue;

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        IntRectangle rectangle = XnaExtensions.ToRectangle(
                            x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                        var tile = grid.GetTile(context.Entities, i, _tilesetAssetsCache.Length, x - grid.Origin.X, y - grid.Origin.Y);

                        // Draw the individual tiles
                        if (tile.tile >= 0)
                        {
                            if (asset.TargetBatch != ((int)Batches2D.FloorBatchId))
                            {
                                asset.DrawTile(
                                    render.GetBatch((int)asset.TargetBatch),
                                        rectangle.X - Grid.HalfCellSize, rectangle.Y - Grid.HalfCellSize,
                                        tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                                    1f, Color.White,
                                    RenderServices.BLEND_NORMAL, tile.sortAdjust);
                            }

                            for (int j = 0; j < asset.AdditionalTiles.Length; j++)
                            {
                                var guid = asset.AdditionalTiles[j];
                                var additionalTile = Game.Data.GetAsset<TilesetAsset>(guid);
                                additionalTile.DrawTile(
                                    render.GetBatch((int)additionalTile.TargetBatch),
                                    rectangle.X - Grid.HalfCellSize, rectangle.Y - Grid.HalfCellSize,
                                    tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                                    1f, Color.White,
                                RenderServices.BLEND_NORMAL, tile.sortAdjust - 1);
                            }

                        }
                    }
                }
            }
        }
    }

}