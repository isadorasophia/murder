using Bang.Contexts;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Graphics
{
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TilesetComponent), typeof(MapComponent))]
    public class TilemapRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<TilesetComponent>() is not TilesetComponent themeComponent || 
                context.World.TryGetUnique<MapComponent>() is not MapComponent mapComponent || mapComponent.Map == null)
            {
                // TODO: This is done so we don't crash the editor.
                // Should we just disable this system instead?
                return default;
            }

            if (themeComponent.Tileset == Guid.Empty)
            {
                // Nothing to be drawn.
                return default;
            }

            TilesetAsset tilemap = Game.Data.GetAsset<TilesetAsset>(themeComponent.Tileset);
            var floorFrames = Game.Data.GetAsset<AsepriteAsset>(themeComponent.Floor).Animations[string.Empty].Frames;

            Map map = mapComponent.Map;
            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    MapTile tile = map.GetGridMap(x, y);

                    Color color = Color.White;
                    Microsoft.Xna.Framework.Vector3 blend = RenderServices.BlendNormal;

                    IntRectangle rectangle = XnaExtensions.ToRectangle(
                        x * Grid.CellSize - Grid.HalfCell, y * Grid.CellSize - Grid.HalfCell, Grid.CellSize, Grid.CellSize);

                    var noise = NoiseHelper.Simple2D(x, y);
                    var floor = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(floorFrames[Calculator.RoundToInt(noise * (floorFrames.Length-1))]);

                    floor.Draw(render.FloorSpriteBatch, rectangle.Center, 0f, Color.White, 1, RenderServices.BlendNormal);

                    bool topLeft = map.HasStaticCollision(x - 1, y - 1);
                    bool topRight = map.HasStaticCollision(x, y - 1);
                    bool botLeft = map.HasStaticCollision(x - 1, y);
                    bool botRight = map.HasStaticCollision(x, y);

                    tilemap.DrawAutoTile(render.GameplayBatch, rectangle.X, rectangle.Y, topLeft, topRight, botLeft, botRight, 1, Color.Lerp(color,Color.White,0.4f), RenderServices.BlendNormal);
                }
            }

            return default;
        }
    }
}
