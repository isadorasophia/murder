﻿using Murder.Core.Geometry;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Services
{
    public static class GeometryServices
    {
        private static readonly Dictionary<String, Vector2[]> _circleCache = new();
        private static readonly Dictionary<String, Vector2[]> _flatCircleCache = new();

        /// <summary>
        /// Creates a list of vectors that represents a circle
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <returns>A list of vectors that, if connected, will create a circle</returns>
        public static Vector2[] CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = $"{radius}x{sides}";
            if (_circleCache.ContainsKey(circleKey))
            {
                return _circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // Cache this circle so that it can be quickly drawn next time
            var result = vectors.ToArray();
            _circleCache.Add(circleKey, result);

            return result;
        }

        public static Vector2[] CreateOrGetFlatenedCircle(float radius, float scaleY, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = $"{radius}x{scaleY}x{sides}";
            if (_flatCircleCache.ContainsKey(circleKey))
            {
                return _flatCircleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)) * scaleY));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0)) * scaleY));

            // Cache this circle so that it can be quickly drawn next time
            var result = vectors.ToArray();
            _flatCircleCache.Add(circleKey, result);

            return result;
        }

        /// <summary>
        /// Check if two ranges overlap at any point.
        /// </summary>
        public static bool CheckOverlap(float minA, float maxA, float minB, float maxB)
        {
            return minA <= maxB && minB <= maxA;
        }

        /// <summary>
        /// Check if two ranges overlap at any point.
        /// </summary>
        public static bool CheckOverlap((float Min, float Max) a, (float Min, float Max) b)
        {
            return a.Min <= b.Max && b.Min <= a.Max;
        }

        /// <summary>
        /// Calculates the signed area of a polygon.
        /// The signed area is positive if the vertices are in clockwise order,
        /// and negative if the vertices are in counterclockwise order.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static float SignedPolygonArea(Vector2[] vertices)
        {
            // Add the first vertex to the end of the array.
            var points = vertices.Concat(new[] { vertices[0] }).ToArray();

            // Calculate the signed area using the Shoelace formula.
            float area = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                area += (points[i + 1].X - points[i].X) * (points[i + 1].Y + points[i].Y);
            }
            return area / 2;
        }

        /// <summary>
        /// Determines if a polygon is convex or not.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="isClockwise"></param>
        /// <returns></returns>
        public static bool IsConvex(Vector2[] vertices, bool isClockwise)
        {
            // Add the first vertex to the end of the array.
            var points = vertices.Concat(new[] { vertices[0] }).ToArray();

            // Check the internal angles of the polygon.
            float angle = 0;
            for (int i = 0; i < vertices.Length - 2; i++)
            {
                // Calculate the internal angle of the polygon.
                Vector2 a = points[i];
                Vector2 b = points[i + 1];
                Vector2 c = points[i + 2];
                float angleSign = isClockwise ? 1 : -1;
                angle += angleSign * Vector2Helper.CalculateAngle(a, b, c);

                // Check if the angle is greater than 180 degrees.
                if (Math.Abs(angle) > MathF.PI)
                {
                    return false;
                }
            }

            // Return true if all angles are less than 180 degrees.
            return true;
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="x">The X position of the point to check.</param>
        /// <param name="y">The Y position of the point to check.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(float x, float y, Rectangle rect)
        {
            if (x <= rect.X) return false;
            if (x >= rect.X + rect.Width) return false;
            if (y <= rect.Y) return false;
            if (y >= rect.Y + rect.Height) return false;
            return true;
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="x">The X position of the point to check.</param>
        /// <param name="y">The Y position of the point to check.</param>
        /// <param name="rx">The left of the rectangle.</param>
        /// <param name="ry">The top of the rectangle.</param>
        /// <param name="rw">The width of the rectangle.</param>
        /// <param name="rh">The height of the rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(float x, float y, float rx, float ry, float rw, float rh)
        {
            if (x <= rx) return false;
            if (x >= rx + rw) return false;
            if (y <= ry) return false;
            if (y >= ry + rh) return false;
            return true;
        }

        public static float RoundedDecimals(float x)
        {
            return x - MathF.Floor(x);
        }

        public static float Decimals(float x)
        {
            return x - MathF.Floor(x);
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="xy">The X and Y position of the point to check.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(Vector2 xy, Rectangle rect)
        {
            return InRect((float)xy.X, (float)xy.Y, rect);
        }

        /// <summary>
        /// Distance check.
        /// </summary>
        /// <param name="x1">The first X position.</param>
        /// <param name="y1">The first Y position.</param>
        /// <param name="x2">The second X position.</param>
        /// <param name="y2">The second Y position.</param>
        /// <returns>The distance between the two points.</returns>
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        /// <summary>
        /// Find the distance between a point and a rectangle.
        /// </summary>
        /// <param name="px">The X position of the point.</param>
        /// <param name="py">The Y position of the point.</param>
        /// <param name="rx">The X position of the rectangle.</param>
        /// <param name="ry">The Y position of the rectangle.</param>
        /// <param name="rw">The width of the rectangle.</param>
        /// <param name="rh">The height of the rectangle.</param>
        /// <returns>The distance.  Returns 0 if the point is within the rectangle.</returns>
        public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
        {
            if (px >= rx && px <= rx + rw)
            {
                if (py >= ry && py <= ry + rh) return 0;
                if (py > ry) return py - (ry + rh);
                return ry - py;
            }
            if (py >= ry && py <= ry + rh)
            {
                if (px > rx) return px - (rx + rw);
                return rx - px;
            }
            if (px > rx)
            {
                if (py > ry) return Distance(px, py, rx + rw, ry + rh);
                return Distance(px, py, rx + rw, ry);
            }
            if (py > ry) return Distance(px, py, rx, ry + rh);
            return Distance(px, py, rx, ry);
        }
        public static bool IntersectsCircle(this Rectangle rectangle, Vector2 circleCenter, float circleRadiusSquared)
        {
            // shift coordinate system to rectangle center
            var diff = rectangle.Center - circleCenter;
            // fold circle into positive quadrant
            var diffPositive = new Vector2(
                MathF.Abs(diff.X),
                MathF.Abs(diff.Y)
                );
            // shift coordinate system to rectangle corner
            var closest = diffPositive - rectangle.Size / 2f;
            // set negative coordinates to 0
            // so they do not affect the check below
            var closestPositive = new Vector2(
                MathF.Max(closest.X, 0),
                MathF.Max(closest.Y, 0)
                );
            // perform distance check
            return closestPositive.LengthSquared() <= circleRadiusSquared;
        }

        /// <summary>
        /// Distance between a line and a point.
        /// </summary>
        /// <param name="x">The X position of the point.</param>
        /// <param name="y">The Y position of the point.</param>
        /// <param name="x1">The first X position of the line.</param>
        /// <param name="y1">The first Y position of the line.</param>
        /// <param name="x2">The second X position of the line.</param>
        /// <param name="y2">The second Y position of the line.</param>
        /// <returns>The distance from the point to the line.</returns>
        public static float DistanceLinePoint(float x, float y, float x1, float y1, float x2, float y2)
        {
            if (x1 == x2 && y1 == y2) return Distance(x, y, x1, y1);

            var px = x2 - x1;
            var py = y2 - y1;

            float something = px * px + py * py;

            var u = ((x - x1) * px + (y - y1) * py) / something;

            if (u > 1) u = 1;
            if (u < 0) u = 0;

            var xx = x1 + u * px;
            var yy = y1 + u * py;

            var dx = xx - x;
            var dy = yy - y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static Rectangle Shrink(this Rectangle rectangle, int amount) => new(
            rectangle.X + amount,
            rectangle.Y + amount,
            rectangle.Width - amount * 2,
            rectangle.Height - amount * 2);
        
        public static Vector2 PointInCircleEdge(float percent)
        {
            var angle = percent * Math.PI * 2;
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Returns the area of <paramref name="b"/> that does not interlap with <paramref name="a"/>.
        /// </summary>
        public static IList<Rectangle> GetOuterIntersection(Rectangle a, Rectangle b)
        {
            List<Rectangle> rectangles = new();

            // Check for an area of b above a
            if (b.Top < a.Top)
            {
                rectangles.Add(new Rectangle
                {
                    X = b.X,
                    Y = b.Y,
                    Width = b.Width,
                    Height = a.Top - b.Top
                });
            }

            // Check for an area of b below a
            if (b.Bottom > a.Bottom)
            {
                rectangles.Add(new Rectangle
                {
                    X = b.X,
                    Y = a.Bottom,
                    Width = b.Width,
                    Height = b.Bottom - a.Bottom
                });
            }

            // Check for an area of b to the left of a
            if (b.Left < a.Left)
            {
                float top = Math.Max(b.Top, a.Top);
                float bottom = Math.Min(b.Bottom, a.Bottom);

                rectangles.Add(new Rectangle
                {
                    X = b.X,
                    Y = top,
                    Width = a.Left - b.Left,
                    Height = bottom - top
                });
            }

            // Check for an area of b to the right of a
            if (b.Right > a.Right)
            {
                float top = Math.Max(b.Top, a.Top);
                float bottom = Math.Min(b.Bottom, a.Bottom);

                rectangles.Add(new Rectangle
                {
                    X = a.Right,
                    Y = top,
                    Width = b.Right - a.Right,
                    Height = bottom - top
                });
            }

            return rectangles;
        }

        /// <summary>
        /// Checks whether <paramref name="startPosition"/>, with <paramref name="size"/>, 
        /// fits in <paramref name="area"/> given an <paramref name="endPosition"/>.
        /// </summary>
        public static bool IsValidPosition(IntRectangle[] area, Vector2 startPosition, Point endPosition, Point size)
        {
            bool valid = false;

            Rectangle newRectangleArea = new Rectangle(endPosition, size);

            // Check whether we are within any of the colliders.
            foreach (IntRectangle collider in area)
            {
                if (collider.Contains(newRectangleArea))
                {
                    valid = true;
                }
            }

            if (!valid)
            {
                Vector2 previousPositionVector = startPosition;
                IList<Rectangle> diff = GeometryServices.GetOuterIntersection(new Rectangle(previousPositionVector, size), newRectangleArea);

                foreach (IntRectangle collider in area)
                {
                    for (int i = 0; i < diff.Count; ++i)
                    {
                        if (collider.Contains(diff[i]))
                        {
                            diff.RemoveAt(i);
                            break;
                        }
                    }
                }

                valid = diff.Count == 0;
            }

            return valid;
        }
    }
}
