using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Serialization;
using Murder.Services;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Utilities
{
    public static class GridHelper
    {
        internal static IEnumerable<Point> Area(int cx, int cy, int radius)
        {
            var center = new Point(radius, radius);

            for (int x = 0; x <= radius * 2; x++)
            {
                for (int y = 0; y <= radius * 2; y++)
                {
                    yield return new(x + cx - radius, y + cy - radius);
                }
            }
        }

        internal static IEnumerable<(int x, int y)> Radius(int cx, int cy, int radius)
        {
            var center = new Point(radius, radius);

            for (int x = 0; x <= radius * 2; x++)
            {
                for (int y = 0; y <= radius * 2; y++)
                {
                    if (Calculator.ManhattanDistance(center, new Point(x, y)) < radius)
                    {
                        yield return (x + cx - radius, y + cy - radius);
                    }
                }
            }
        }

        public static IEnumerable<Point> Circle(int cx, int cy, int radius)
        {
            var radiusSquared = radius * radius;

            for (int x = 0; x <= radius * 2; x++)
            {
                for (int y = 0; y <= radius * 2; y++)
                {
                    if (new Rectangle(Calculator.RoundToInt(x), Calculator.RoundToInt(y), 1, 1).IntersectsCircle(new Vector2(radius, radius), radiusSquared))
                    {
                        yield return new(x + cx - radius, y + cy - radius);
                    }
                }
            }
        }

        private static (int grid, float relative) ClampPosition(int grid, float relative, float deltaRelative)
        {
            relative += deltaRelative;

            int overflow = (int)Math.Floor(Math.Abs(relative));

            if (relative > 1)
            {
                relative -= overflow;
                grid += overflow;
            }
            else if (relative < 0)
            {
                relative += overflow;
                grid -= overflow;
            }

            return (grid, relative);
        }

        public static Point ToGrid(this Point position) =>
            new Point(Calculator.FloorToInt(position.X / Grid.CellSize), Calculator.FloorToInt(position.Y / Grid.CellSize));

        public static Point ToGrid(this Vector2 position) =>
            new Point(Calculator.FloorToInt(position.X / Grid.CellSize), Calculator.FloorToInt(position.Y / Grid.CellSize));

        public static IMurderTransformComponent SnapToGridDelta(this IMurderTransformComponent transform)
        {
            return transform.With(
                transform.X - transform.X % Grid.CellSize,
                transform.Y - transform.Y % Grid.CellSize);
        }

        public static Vector2 SnapToGridDelta(this Vector2 vector2)
        {
            return new(
                vector2.X - vector2.X % Grid.CellSize,
                vector2.Y - vector2.Y % Grid.CellSize);
        }

        public static Point SnapToGridDelta(this Point point)
        {
            return new(
                point.X - point.X % Grid.CellSize,
                point.Y - point.Y % Grid.CellSize);
        }

        public static IEnumerable<Point> Line(Point start, Point end)
        {
            int x = start.X;
            int y = start.Y;
            int x2 = end.X;
            int y2 = end.Y;
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                yield return new(x, y);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public static ComplexDictionary<Point, Point> Reverse(this IDictionary<Point, Point> input, Point initial, Point target)
        {
            ComplexDictionary<Point, Point> reversePath = [];

            Point next = target;
            while (next != initial)
            {
                Point previous = input[next];
                reversePath[previous] = next;

                next = previous;
            }

            return reversePath;
        }

        public static IntRectangle GetBoundingBox(this Rectangle rect)
        {
            int top = Grid.FloorToGrid(rect.Top);
            int left = Grid.FloorToGrid(rect.Left);

            int right = Grid.CeilToGrid(rect.Right);
            int bottom = Grid.CeilToGrid(rect.Bottom);

            return new(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Returns all the neighbours of a position.
        /// </summary>
        public static ReadOnlySpan<Point> Neighbours(this Point p, int width, int height, bool includeDiagonals = false)
            => p.Neighbours(0, 0, width, height, includeDiagonals);

        /// <summary>
        /// Returns all the neighbours of a position.
        /// </summary>
        public static ReadOnlySpan<Point> Neighbours(this Point p, int x, int y, int edgeX, int edgeY, bool includeDiagonals = false)
        {
            int index = 0;
            Span<Point> result = new Point[8];

            // [ ] [x] [ ]
            // [ ]  x  [ ]
            // [ ] [ ] [ ]
            if (p.Y > y)
            {
                result[index++] = new Point(p.X, p.Y - 1);
            }

            // [ ] [ ] [ ]
            // [x]  x  [ ]
            // [ ] [ ] [ ]
            if (p.X > x)
            {
                result[index++] = new Point(p.X - 1, p.Y);
            }

            // [ ] [ ] [ ]
            // [ ]  x  [x]
            // [ ] [ ] [ ]
            if (p.X + 1 < edgeX)
            {
                result[index++] = new Point(p.X + 1, p.Y);
            }

            // [ ] [ ] [ ]
            // [ ]  x  [ ]
            // [ ] [x] [ ]
            if (p.Y + 1 < edgeY)
            {
                result[index++] = new Point(p.X, p.Y + 1);
            }

            if (includeDiagonals)
            {
                // [x] [ ] [ ]
                // [ ]  x  [ ]
                // [ ] [ ] [ ]
                if (p.X > x && p.Y > y)
                {
                    result[index++] = new Point(p.X - 1, p.Y - 1);
                }

                // [ ] [ ] [x]
                // [ ]  x  [ ]
                // [ ] [ ] [ ]
                if (p.X + 1 < edgeX && p.Y > y)
                {
                    result[index++] = new Point(p.X + 1, p.Y - 1);
                }

                // [ ] [ ] [ ]
                // [ ]  x  [ ]
                // [x] [ ] [ ]
                if (p.Y + 1 < edgeY && p.X > x)
                {
                    result[index++] = new Point(p.X - 1, p.Y + 1);
                }

                // [ ] [ ] [ ]
                // [ ]  x  [ ]
                // [ ] [ ] [x]
                if (p.X + 1 < edgeX && p.Y + 1 < edgeY)
                {
                    result[index++] = new Point(p.X + 1, p.Y + 1);
                }
            }

            return result.Slice(0, index);
        }

        /// <summary>
        /// Creates a rectangle from <paramref name="p1"/> to <paramref name="p2"/>.
        /// </summary>
        public static IntRectangle FromTopLeftToBottomRight(Point p1, Point p2)
        {
            int left = Math.Min(p1.X, p2.X);
            int top = Math.Min(p1.Y, p2.Y);

            int bottom = Math.Max(p1.Y, p2.Y);
            int right = Math.Max(p1.X, p2.X);

            return new(x: left, y: top, width: right - left + 1, height: bottom - top + 1);
        }

        /// <summary>
        /// Creates a rectangle from <paramref name="p1"/> to <paramref name="p2"/>.
        /// </summary>
        public static Rectangle FromTopLeftToBottomRight(Vector2 p1, Vector2 p2)
        {
            float left = Math.Min(p1.X, p2.X);
            float top = Math.Min(p1.Y, p2.Y);

            float bottom = Math.Max(p1.Y, p2.Y);
            float right = Math.Max(p1.X, p2.X);

            return new(x: left, y: top, width: right - left + 1, height: bottom - top + 1);
        }

        public static IntRectangle GetCarveBoundingBox(this Rectangle rect, float occupiedThreshold = .3f)
        {
            int left, top, right, bottom;

            // Left
            float leftGrid = Grid.FloorToGrid(rect.Left);
            float leftRemainder = rect.Left / Grid.CellSize - leftGrid;
            left = (leftRemainder <= occupiedThreshold) ? Grid.FloorToGrid(rect.Left) : Grid.CeilToGrid(rect.Left);

            // Top
            float topGrid = Grid.FloorToGrid(rect.Top);
            float topRemainder = rect.Top / Grid.CellSize - topGrid;
            top = (topRemainder <= occupiedThreshold) ? Grid.FloorToGrid(rect.Top) : Grid.CeilToGrid(rect.Top);

            // Right
            float rightGrid = Grid.CeilToGrid(rect.Right);
            float rightRemainder = rightGrid - rect.Right / Grid.CellSize;
            right = (rightRemainder <= occupiedThreshold) ? Grid.CeilToGrid(rect.Right) : Grid.FloorToGrid(rect.Right);

            // Bottom
            float bottomGrid = Grid.CeilToGrid(rect.Bottom);
            float bottomRemainder = bottomGrid - rect.Bottom / Grid.CellSize;
            bottom = (bottomRemainder <= occupiedThreshold) ? Grid.CeilToGrid(rect.Bottom) : Grid.FloorToGrid(rect.Bottom);

            return new IntRectangle(left, top, right - left, bottom - top);
        }

        public static Rectangle ToRectangle(Point grid)
        {
            return new Rectangle(grid.X * Grid.CellSize, grid.Y * Grid.CellSize, Grid.CellSize, Grid.CellSize);
        }
    }
}