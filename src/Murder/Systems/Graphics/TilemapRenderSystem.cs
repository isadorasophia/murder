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

                ImmutableArray<string> floorFrames = floorAsset.Animations[string.Empty].Frames;

                TileGridComponent gridComponent = e.GetTileGrid();
                (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(gridComponent.Rectangle);

                TileGrid grid = gridComponent.Grid;
                TilesetAsset[] assets = tilesetComponent.Tilesets.ToAssetArray<TilesetAsset>();
                
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        Color color = Color.White;
                        Microsoft.Xna.Framework.Vector3 blend = RenderServices.BLEND_NORMAL;

                        IntRectangle rectangle = XnaExtensions.ToRectangle(
                            x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                        // TODO: Remove this! Temporary debug!
                        // if (grid.IsSolid(x,y)) RenderServices.DrawRectangleOutline(render.GameplayBatch, rectangle.Expand(-1), color.WithAlpha(.5f));

                        if (x != maxX && y != maxY)
                        {
                            var noise = Calculator.RoundToInt(NoiseHelper.Simple2D(x, y) * (floorFrames.Length - 1));
                            AtlasTexture floor = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(floorFrames[noise]);

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
                            int tileMask = i.ToMask();

                            bool topLeft = GetTileAt(context.Entities, grid, x - 1, y - 1, tileMask);
                            bool topRight = GetTileAt(context.Entities, grid, x, y - 1, tileMask);
                            bool botLeft = GetTileAt(context.Entities, grid, x - 1, y, tileMask);
                            bool botRight = GetTileAt(context.Entities, grid, x, y, tileMask);

                            assets[i].DrawAutoTile(
                                render, rectangle.X - Grid.HalfCell, rectangle.Y - Grid.HalfCell,
                                topLeft, topRight, botLeft, botRight, 1, Color.Lerp(color, Color.White, 0.4f), RenderServices.BLEND_NORMAL);
                        }
                    }
                }
            }
            
            return;
        }

        // TODO: [PERF] This can be heavily optimized if needed 
        private static bool GetTileAt(ImmutableArray<Entity> tilemaps, TileGrid grid, int x, int y, int tileMask)
        {
            bool isOffLimits = x - grid.Origin.X < 0 || x - grid.Origin.X >= grid.Width || y - grid.Origin.Y < 0 || y - grid.Origin.Y >= grid.Height;

            if (!isOffLimits)
            {
                // Easy scecnario, just look for current grid.
                return grid.HasFlagAtGridPosition(x, y, tileMask);
            }

            // Since this is not in the current room, we should look if it's in the other tilemap rooms.
            for (int i = 0; i < tilemaps.Length; i++)
            {
                if (tilemaps[i].GetTileGrid().Grid.HasFlagAtGridPosition(x, y, tileMask))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
