using Bang;
using Bang.Entities;
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
using Murder.Serialization;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(typeof(ITransformComponent), typeof(RouteComponent))]
    public class DebugRouteSystem : IMurderRenderSystem
    {
        private readonly HashSet<Point> _loopCheck = new();
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
                ComplexDictionary<Point, Point> path = e.GetRoute().Nodes;

                Point position = e.GetGlobalTransform().Point;
                Point gridPosition = position.ToGrid();

                if (e.TryGetPathfindStatus() is PathfindStatusComponent pathfindStatusComponent)
                {
                    if (e.TryGetPathfind() is PathfindComponent pathfindComponent)
                    {
                        RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont,
                            $"Entity {e.EntityId}\n\n{pathfindComponent.Algorithm}\n\n{pathfindStatusComponent.Flags}", position, new DrawInfo(Game.Profile.Theme.Accent, 0)
                            {
                                Outline = Game.Profile.Theme.Bg,
                            });
                    }
                }
                else if (e.TryGetPathfind() is PathfindComponent pathfindComponent)
                {
                    RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont,
                        $"Entity {e.EntityId}\n\n{pathfindComponent.Algorithm}", position, new DrawInfo(Game.Profile.Theme.Accent, 0)
                        {
                            Outline = Game.Profile.Theme.Bg,
                        });
                }
                else
                {
                    RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont,
                        $"Entity {e.EntityId} [NO PATHFIND COMPONENT!]", position, new DrawInfo(Game.Profile.Theme.Accent, 0)
                        {
                            Outline = Game.Profile.Theme.Bg,
                        });
                }

                _loopCheck.Clear();
                if (path.ContainsKey(gridPosition))
                {
                    DrawPoint(render, path, gridPosition, _loopCheck);
                }
                else
                {
                    // Look for the closest cell to start drawing the path.
                    Point closest = path.Keys.OrderBy(p => (gridPosition - p).LengthSquared()).FirstOrDefault();
                    DrawPoint(render, path, gridPosition, _loopCheck);
                }
                
                foreach (var point in path.Values)
                {
                    if (_loopCheck.Contains(point))
                    {
                        RenderServices.DrawRectangleOutline(
                            render.DebugBatch, new Rectangle(point.FromCellToPointPosition() + offset, rectSize),
                            Game.Profile.Theme.HighAccent * 0.5f);
                    }
                    else
                    {
                        RenderServices.DrawRectangle(
                            render.DebugBatch, new Rectangle(point.FromCellToPointPosition() + offset, rectSize),
                            Game.Profile.Theme.HighAccent * 0.5f);
                    }
                }

            }

            Map map = context.World.GetUniqueMap().Map;
            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);

            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (hook.DrawQuadTree == EditorHook.ShowQuadTree.Pathfind)
            {
                DrawQuadtree(render, context, map, minX, maxX, minY, maxY);
            }
        }

        private void DrawPoint(RenderContext render, ComplexDictionary<Point, Point> path, Point point, HashSet<Point> loopCheck)
        {
            if (path.TryGetValue(point, out Point next))
            {
                if (!loopCheck.Add(point))
                {
                    return;
                }

                Point center = point.FromCellToPointPosition() + Grid.HalfCellDimensions;
                Point nextCenter = next.FromCellToPointPosition() + Grid.HalfCellDimensions;

                RenderServices.DrawLine(render.DebugBatch, center, nextCenter, Game.Profile.Theme.Accent, 2, 0.8f);

                DrawPoint(render, path, next, loopCheck);
            }
        }

        private void DrawQuadtree(RenderContext render, Context context, Map map,
            int minX, int maxX, int minY, int maxY)
        {
            Color nodeColor = new(.8f, .5f, .1f, .1f);

            if (context.World.TryGetUniqueHAAStarPathfind()?.Data is HAAStar pathfind)
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

                    RenderServices.DrawCircleOutline(
                        render.DebugBatch, center,
                        radius: Grid.HalfCellSize, sides: 24, nodeColor);

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