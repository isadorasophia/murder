    using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics
{
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TileGridComponent))]
    public class TilemapRenderSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<TilesetComponent>() is not TilesetComponent tilesetComponent)
            {
                // Skip drawing on empty.
                return;
            }
            
            // Iterate over each room.
            foreach (Entity e in context.Entities)
            {
                if (tilesetComponent.Tilesets.IsEmpty || 
                    e.TryGetRoom()?.Floor is not Guid floorGuid || 
                    Game.Data.TryGetAsset<AsepriteAsset>(floorGuid) is not AsepriteAsset floorAsset)
                {
                    // Nothing to be drawn.
                    continue;
                }

                ImmutableArray<int> floorFrames = floorAsset.Animations[string.Empty].Frames;

                TileGridComponent gridComponent = e.GetTileGrid();
                (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(gridComponent.Rectangle);

                TileGrid grid = gridComponent.Grid;
                TilesetAsset[] assets = tilesetComponent.Tilesets.ToAssetArray<TilesetAsset>();
                
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX+2; x++)
                    {
                        Color color = Color.White;
                        Microsoft.Xna.Framework.Vector3 blend = RenderServices.BLEND_NORMAL;

                        IntRectangle rectangle = XnaExtensions.ToRectangle(
                            x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                        if (x < maxX && y < maxY)
                        {
                            var noise = Calculator.RoundToInt(NoiseHelper.Simple2D(x, y) * (floorFrames.Length - 1));
                            AtlasCoordinates floor = floorAsset.GetFrame(floorFrames[noise]);

                            // Depth layer is set to zero or it will be in the same layer as the editor floor.
                            floor.Draw(
                                render.FloorSpriteBatch,
                                new Point(x, y) * Grid.CellSize,
                                Vector2.One,
                                Vector2.Zero,
                                0f,
                                ImageFlip.None,
                                Color.White,
                                RenderServices.BLEND_NORMAL,
                                0.8f);
                        }

                        for (int i = 0; i < assets.Length; ++i)
                        {
                            var tile = grid.GetTile(context.Entities, i, assets.Length, x - grid.Origin.X, y - grid.Origin.Y);
                            // if (tile.tile < 0) tile.tile = 4;

                            if (tile.tile>=0)
                                assets[i].DrawTile(
                                    render.GetSpriteBatch(assets[i].TargetBatch),
                                    rectangle.X - Grid.HalfCell, rectangle.Y - Grid.HalfCell,
                                    tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                                    1f, Color.Lerp(color, Color.White, 0.4f),
                                    RenderServices.BLEND_NORMAL, tile.sortAdjust);
                        }
                    }
                }
            }
            
            return;
        }

    }
}
