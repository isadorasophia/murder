using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

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
                            x * Grid.CellSize - Grid.HalfCell, y * Grid.CellSize - Grid.HalfCell, Grid.CellSize, Grid.CellSize);

                        var noise = NoiseHelper.Simple2D(x, y);
                        var floor = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(floorFrames[Calculator.RoundToInt(noise * (floorFrames.Length - 1))]);

                        floor.Draw(render.FloorSpriteBatch, rectangle.Center, 0f, Color.White, 1, RenderServices.BlendNormal);

                        bool topLeft = grid.IsSolid(x - 1, y - 1);
                        bool topRight = grid.IsSolid(x, y - 1);
                        bool botLeft = grid.IsSolid(x - 1, y) ;
                        bool botRight = grid.IsSolid(x, y);

                        tilemap.DrawAutoTile(render.GameplayBatch, rectangle.X, rectangle.Y, topLeft, topRight, botLeft, botRight, 1, Color.Lerp(color, Color.White, 0.4f), RenderServices.BlendNormal);
                    }
                }
            }
            
            return default;
        }
    }
}
