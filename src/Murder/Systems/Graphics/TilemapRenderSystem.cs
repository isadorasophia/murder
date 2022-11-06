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
using System.Diagnostics;

namespace Murder.Systems.Graphics
{
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TileGridComponent))]
    public class TilemapRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                TilesetComponent tilesetComponent = e.GetTileset();
                if (Game.Data.TryGetAsset<TilesetAsset>(tilesetComponent.Tileset) is not TilesetAsset tilemap ||
                    Game.Data.TryGetAsset<AsepriteAsset>(tilesetComponent.Floor) is not AsepriteAsset floorAsset)
                {
                    // Nothing to be drawn.
                    return default;
                }

                ImmutableArray<string> floorFrames = floorAsset.Animations[string.Empty].Frames;

                TileGridComponent gridComponent = e.GetTileGrid();
                (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(gridComponent.Rectangle);

                TileGrid grid = gridComponent.Grid;
                for (int y = minY; y < maxY; y++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        Color color = Color.White;
                        Microsoft.Xna.Framework.Vector3 blend = RenderServices.BlendNormal;

                        IntRectangle rectangle = XnaExtensions.ToRectangle(
                            x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                        // TODO: Remove this! Temporary debug!
                        // if (grid.IsSolid(x,y)) RenderServices.DrawRectangleOutline(render.GameplayBatch, rectangle.Expand(-1), color.WithAlpha(.5f));

                        var noise = NoiseHelper.Simple2D(x, y);
                        AtlasTexture floor = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(floorFrames[Calculator.RoundToInt(noise * (floorFrames.Length - 1))]);

                        // Depth layer is set to zero or it will be in the same layer as the editor floor.
                        floor.Draw(render.FloorSpriteBatch, new Point(x, y) * Grid.CellSize, 0f, Color.White, depthLayer: 0, RenderServices.BlendNormal);

                        bool topLeft = grid.IsSolidAtGridPosition(x - 1, y - 1);
                        bool topRight = grid.IsSolidAtGridPosition(x, y - 1);
                        bool botLeft = grid.IsSolidAtGridPosition(x - 1, y);
                        bool botRight = grid.IsSolidAtGridPosition(x, y);

                        tilemap.DrawAutoTile(render.GameplayBatch, rectangle.X - Grid.HalfCell, rectangle.Y + Grid.HalfCell, topLeft, topRight, botLeft, botRight, 1, Color.Lerp(color, Color.White, 0.4f), RenderServices.BlendNormal);
                    }
                }
            }
            
            return default;
        }
    }
}
