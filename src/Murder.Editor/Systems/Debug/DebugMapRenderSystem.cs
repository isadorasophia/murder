using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(MapComponent))]
    internal class DebugMapRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            var editorHook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (!editorHook.DrawGrid && !editorHook.DrawCollisions)
            {
                return;
            }
            
            if (context.World.TryGetUnique<MapComponent>()?.Map is not Map map)
            {
                return;
            }

            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    Rectangle tileRectangle = XnaExtensions.ToRectangle(
                        x * Grid.CellSize - Grid.HalfCell, y * Grid.CellSize - Grid.HalfCell, Grid.CellSize, Grid.CellSize);

                    int topLeft = map.GetCollision(x - 1, y - 1);
                    int topRight = map.GetCollision(x, y - 1);
                    int botLeft = map.GetCollision(x - 1, y);
                    int botRight = map.GetCollision(x, y);

                    Color solidGridColor = new(.1f, .9f, .9f, .4f);
                    Color solidCarveColor = new(.9f, .2f, .8f, .4f);
                    Color gridColor = new(.2f, .9f, .1f, .05f);

                    Rectangle cellRectangle = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                    if (editorHook.DrawGrid)
                    {
                        float sorting = 1;

                        render.DebugSpriteBatch.DrawRectangleOutline(cellRectangle, gridColor, 1, sorting);
                    }

                    if (editorHook.DrawCollisions)
                    {
                        DrawTileCollisions(topLeft, topRight, botLeft, botRight, render, tileRectangle, solidGridColor);
                        DrawCarveCollision(botRight, render, cellRectangle, solidCarveColor);
                    }
                }
            }
        }

        private void DrawTileCollisions(int topLeft, int topRight, int botLeft, int botRight,
            RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.SOLID;

            if ((topLeft & mask) != 0)
            {
                render.DebugSpriteBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y, Grid.HalfCell, Grid.HalfCell), color, sorting);
            }

            if ((topRight & mask) != 0)
            {
                render.DebugSpriteBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCell, rectangle.Y, Grid.HalfCell, Grid.HalfCell), color, sorting);
            }

            if ((botLeft & mask) != 0)
            {
                render.DebugSpriteBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + Grid.HalfCell, Grid.HalfCell, Grid.HalfCell), color, sorting);
            }

            if ((botRight & mask) != 0)
            {
                render.DebugSpriteBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCell, rectangle.Y + Grid.HalfCell, Grid.HalfCell, Grid.HalfCell), color, sorting);
            }
        }

        private void DrawCarveCollision(int cell, RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.CARVE;

            if ((cell & mask) != 0)
            {
                render.DebugSpriteBatch.DrawRectangle(rectangle, color, sorting);
            }
        }
    }
}
