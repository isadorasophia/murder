using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics;


/// <summary>
/// Generic and all-around tilemap rendering system. Draws all tilesmaps and floor tiles that are visible to the camera.
/// </summary>
[EditorSystem]
[Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TileGridComponent))]
public class TilemapAndFloorRenderSystem : IMurderRenderSystem
{
    private bool ShowDebugOcclusion => false;

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUniqueTileset() is not TilesetComponent tilesetComponent)
        {
            // Skip drawing on empty.
            return;
        }
        TilesetAsset[] assets = tilesetComponent.Tilesets.ToAssetArray<TilesetAsset>();

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

            SpriteAsset floorSpriteAsset = floorAsset.Image.Asset;
            Texture2D[] floorSpriteAtlas = Game.Data.FetchAtlas(floorSpriteAsset.Atlas).Textures;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    IntRectangle rectangle = XnaExtensions.ToRectangle(
                        x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                    bool occluded = false;
                    for (int i = 0; i < assets.Length; ++i)
                    {
                        var tile = grid.GetTile(context.Entities, i, assets.Length, x - grid.Origin.X, y - grid.Origin.Y);

                        // Draw the individual tiles
                        if (tile.tile >= 0)
                        {
                            var asset = assets[i];
                            if (asset == null)
                                continue;

                            asset.DrawTile(
                            render.GetBatch((int)asset.TargetBatch),
                                rectangle.X - Grid.HalfCellSize, rectangle.Y - Grid.HalfCellSize,
                                tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                            1f, Color.White,
                            RenderServices.BLEND_NORMAL, tile.sortAdjust);

                            for (int j = 0; j < asset.AdditionalTiles.Length; j++)
                            {
                                var guid = asset.AdditionalTiles[j];
                                var additionalTile = Game.Data.GetAsset<TilesetAsset>(guid);
                                additionalTile.DrawTile(
                                    render.GetBatch((int)additionalTile.TargetBatch),
                                    rectangle.X - Grid.HalfCellSize, rectangle.Y - Grid.HalfCellSize,
                                    tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                                    1f, Color.White,
                                RenderServices.BLEND_NORMAL, tile.sortAdjust);
                            }

                        }
                        if (tile.occludeGround)
                            occluded = true;
                    }

                    // Debug test for occluded floor
#if DEBUG
                    if (ShowDebugOcclusion)
                    {
                        RenderServices.DrawRectangleOutline(render.GameplayBatch, new Rectangle(x, y, 1, 1) * Grid.CellSize, Color.Magenta, 2, 0f);
                    }
#endif

                    // Draw the actual floor
                    if (floorSpriteAsset is not null && (floorAsset.AlwaysDraw || !occluded) && x < maxX && y < maxY)
                    {
                        ImmutableArray<int> floorFrames = floorSpriteAsset.Animations[string.Empty].Frames;

                        var noise = Calculator.RoundToInt(NoiseHelper.Simple2D(x, y) * (floorFrames.Length - 1));
                        AtlasCoordinates floor = floorSpriteAsset.GetFrame(floorFrames[noise]);

                        // Draw each individual ground tile.
                        render.FloorBatch.Draw(
                            floorSpriteAtlas[floor.AtlasIndex],
                            new Point(x, y) * Grid.CellSize,
                            floor.Size,
                            floor.SourceRectangle,
                            0.8f,
                            0,
                            Microsoft.Xna.Framework.Vector2.One,
                            ImageFlip.None,
                            Color.White,
                            Microsoft.Xna.Framework.Vector2.Zero,
                            RenderServices.BLEND_NORMAL
                            );
                    }
                }
            }
        }

        return;
    }

}