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
using SharpFont;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [WorldEditor(startActive: true)]
    [Filter(kind: ContextAccessorKind.Read, typeof(MapComponent))]
    internal class DebugMapRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            var editorHook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (!editorHook.DrawGrid)
            {
                return;
            }

            Map? map = context.World.TryGetUnique<MapComponent>()?.Map;
            if (map is null)
            {
                return;
            }

            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);
            Color gridColor = Color.CreateFrom256(r: 170, g: 227, b: 12) * .1f;

            for (int y = minY; y < maxY; y++)
            {
                RenderServices.DrawLine(
                    render.DebugBatch,
                    new Point(minX, y) * Grid.CellSize,
                    new Point(maxX, y) * Grid.CellSize,
                    gridColor);

                for (int x = minX; x < maxX; x++)
                {
                    Rectangle tileRectangle = XnaExtensions.ToRectangle(
                        x * Grid.CellSize - Grid.HalfCellSize, y * Grid.CellSize - Grid.HalfCellSize, Grid.CellSize, Grid.CellSize);

                    Rectangle cellRectangle = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                    int topLeft = map.GetCollision(x - 1, y - 1);
                    int topRight = map.GetCollision(x, y - 1);
                    int botLeft = map.GetCollision(x - 1, y);
                    int botRight = map.GetCollision(x, y);

                    Color solidGridColor = new Color(.1f, .9f, .9f) * .4f;
                    Color solidCarveColor = new Color(.9f, .2f, .8f) * .4f;

                    DrawTileCollisions(topLeft, topRight, botLeft, botRight, render, tileRectangle, solidGridColor);
                    DrawCarveCollision(botRight, render, cellRectangle, solidCarveColor);
                }
            }

            for (int x = minX; x < maxX; x++)
            {
                RenderServices.DrawLine(
                    render.DebugBatch,
                    new Point(x, minY) * Grid.CellSize,
                    new Point(x, maxY) * Grid.CellSize,
                    gridColor);
            }
        }

        private void DrawTileCollisions(int topLeft, int topRight, int botLeft, int botRight,
            RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.SOLID;

            if ((topLeft & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((topRight & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCellSize, rectangle.Y, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((botLeft & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + Grid.HalfCellSize, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((botRight & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCellSize, rectangle.Y + Grid.HalfCellSize, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }
        }

        private void DrawCarveCollision(int cell, RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.CARVE;

            if ((cell & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(rectangle, color, sorting);
            }
        }
    }
}