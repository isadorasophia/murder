using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Ai;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(typeof(ITransformComponent), typeof(RouteComponent))]
    public class DebugRouteSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent && !editorComponent.EditorHook.DrawPathfind)
            {
                return;
            }

            var offset = new Point(4, 4);
            var rectSize = Grid.CellDimensions - offset * 2;
            foreach (var e in context.Entities)
            {
                ImmutableDictionary<Point, Point> path = e.GetComponent<RouteComponent>().Nodes;

                foreach (var point in path.Values)
                {
                    RenderServices.DrawRectangle(
                        render.DebugBatch, new Rectangle(point.FromCellToPointPosition() + offset, rectSize),
                        (Game.Profile.Theme.Faded * 0.65f).ToXnaColor());
                }
            }

            Color numberColor = new(.2f, .9f, .1f, .2f);

            Map map = context.World.GetUnique<MapComponent>().Map;
            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    if (map.IsObstacle(new(x, y)))
                    {
                        continue;
                    }

                    RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, $"{map.WeightAt(x, y)}",
                        new(x * Grid.CellSize + Grid.HalfCell, y * Grid.CellSize + Grid.HalfCell + 2),
                        new DrawInfo(0)
                            {
                            Origin = new(0.5f, 0.5f),
                            Color = numberColor,
                            });
                }
            }

            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (hook.DrawQuadTree == EditorHook.ShowQuadTree.Pathfind)
            {
                DrawQuadtree(render, context, map, minX, maxX, minY, maxY);
            }
        }

        private void DrawQuadtree(RenderContext render, Context context, Map map,
            int minX, int maxX, int minY, int maxY)
        {
            Color nodeColor = new(.8f, .5f, .1f, .1f);

            if (context.World.TryGetUnique<HAAStarPathfindComponent>()?.Data is HAAStar pathfind)
            {
                DrawQuadtreeGrid(render, HAAStar.CLUSTER_SIZE, nodeColor, map.Width, map.Height,
                    minX, maxX, minY, maxY);

                foreach ((Point p, HAAStar.Node n) in pathfind.DebugNodes)
                {
                    // Make sure we are within bounds!
                    if (p.X < minX || p.X >= maxX || p.Y < minY || p.Y >= maxY)
                    {
                        continue;
                    }

                    Point center = p.FromCellToPointPosition() + Grid.HalfCellDimensions;

                    RenderServices.DrawCircle(
                        render.DebugBatch, center,
                        radius: Grid.HalfCell, sides: 24, nodeColor);

                    foreach (Point neighbour in n.Neighbours.Keys)
                    {
                        Point target = neighbour.FromCellToPointPosition() + Grid.HalfCellDimensions;
                        RenderServices.DrawLine(render.DebugBatch, center, target, nodeColor);
                    }
                }
            }
        }

        private void DrawQuadtreeGrid(
            RenderContext render, int qtSize, Color color,
            int width, int height, int minX, int maxX, int minY, int maxY)
        {
            minX = Math.Max(0, minX - qtSize);
            minY = Math.Max(0, minY - qtSize);

            minX -= minX % qtSize;
            minY -= minY % qtSize;

            maxX = Math.Min(width, maxX + qtSize);
            maxY = Math.Min(height, maxY + qtSize);

            for (int y = minY; y < maxY; y += qtSize)
            {
                for (int x = minX; x < maxX; x += qtSize)
                {
                    Rectangle rect = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, qtSize * Grid.CellSize, qtSize * Grid.CellSize);

                    render.DebugBatch.DrawRectangleOutline(rect, color);
                }
            }
        }
    }
}
