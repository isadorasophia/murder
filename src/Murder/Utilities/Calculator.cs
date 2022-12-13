using Murder.Core.Geometry;

namespace Murder.Utilities
{
    /// <summary>
    /// Calculator helper class.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Default layers count.
        /// TODO: Make this customizable.
        /// </summary>
        public static int LayersCount = 65536; // 16 bits layers [-32768, 32768]

        public const float TO_DEG = 180 / MathF.PI;
        public const float TO_RAD = MathF.PI / 180;

        #region Lists and Arrays

        /// <summary>
        /// Add <paramref name="item"/> to <paramref name="list"/>. Skip if already present.
        /// Cost O(n).
        /// </summary>
        public static T AddOnce<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }

            return item;
        }

        public static T[] RepeatingArray<T>(T value, int size)
        {
            var array = new T[size];
            array.Populate(value);
            return array;
        }

        public static int OneD(this Point p, int width)
        {
            return p.X + p.Y * width;
        }

        public static int OneD(int x, int y, int width)
        {
            return x + y * width;
        }

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        #endregion

        public static bool Blink(float speed)
        {
            if (speed == 0)
            {
                return false;
            }

            var duration = 1 / speed;
            return MathF.Round(Game.Now * speed) % 2 == 0;
        }

        public static bool SameSign(float num1, float num2)
        {
            return num1 == 0 || num2 == 0 || num1 > 0 && num2 > 0 || num1 < 0 && num2 < 0;
        }

        public static bool SameSignOrSimilar(float num1, float num2)
        {
            return
                num1 == 0 || num2 == 0 ||
                num1 > 0 && num2 > 0 || num1 < 0 && num2 < 0 ||
                MathF.Abs(num1 - num2) < float.Epsilon;
        }

        public static int WrapAround(int value,in int min,in int max)
        {
            if (max < 0)
                return 0;

            while (value < min)
                value += max + 1;
            while (value > max)
                value -= max + 1;

            return value;
        }

        public static float ConvertLayerToLayerDepth(int layer)
        {
            return (LayersCount / 2 - layer) / (float)LayersCount;
        }

        public static int ConvertLayerDepthToLayer(float layerDepth)
        {
            return (LayersCount / 2) - (int)(layerDepth * LayersCount);
        }

        /// <summary>
        /// Takes an elapsed time and coverts it to a 0-1 range
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="maxTime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static float ClampTime(float elapsed, float maxTime)
        {
            return Calculator.Clamp01(Math.Clamp(elapsed, 0, maxTime) / maxTime);
        }

        public static float Approach(float from, float target, float amount)
        {
            if (from > target)
                return Math.Max(from - amount, target);
            else
                return Math.Min(from + amount, target);
        }

        public static Vector2 Approach(in Vector2 from, in Vector2 target, float amount)
        {
            if (from == target)
                return target;

            var diff = target - from;
            if (diff.LengthSquared() <= amount * amount)
                return target;
            else
                return from + diff.Normalized() * amount;
        }

        public static Vector2 Normalized(this in Vector2 vector2)
        {
            var result = vector2;
            result.Normalized();
            return result;
        }

        public static float Clamp01(float v) => Math.Clamp(v, 0.0f, 1.0f);
        public static float Clamp01(int v) => Math.Clamp(v, 0, 1);

        public static float Remap(float input, float inputMin, float inputMax, float min, float max)
        {
            return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
        }
        public static float Lerp(float origin, float target, float factor)
        {
            return origin * (1 - factor) + target * factor;
        }
        public static int LerpInt(float origin, float target, float factor)
        {
            return RoundToInt(origin * (1 - factor) + target * factor);
        }

        public static float LerpSnap(float origin, float target, float factor, float threshold = 0.01f)
        {
            return Math.Abs(target - origin) < threshold ? target : origin * (1 - factor) + target * factor;
        }

        public static int FloorToInt(float v) => (int)MathF.Floor(v);

        public static int CeilToInt(float v) => (int)MathF.Ceiling(v);

        /// <summary>
        /// Rounds and converts a number to integer with <see cref="MathF.Round(float)"/>.
        /// </summary>
        public static int RoundToInt(float v) => (int)MathF.Round(v);

        public static int RoundToEven(float v) => (int)MathF.Round(v / 2, MidpointRounding.AwayFromZero) * 2;

        public static Point ToPoint(this System.Numerics.Vector2 vector) => new Point(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));
        public static System.Numerics.Vector2 ToSysVector2(this Microsoft.Xna.Framework.Point point) => new System.Numerics.Vector2((float)point.X, (float)point.Y);
        public static System.Numerics.Vector2 ToSysVector2(this Microsoft.Xna.Framework.Vector2 vector) => new System.Numerics.Vector2(vector.X, vector.Y);
        public static Vector2 ToXnaVector2(this System.Numerics.Vector2 vector) => new Vector2(vector.X, vector.Y);
        public static Vector2 ToCore(this System.Numerics.Vector2 vector) => new Vector2(vector.X, vector.Y);

        internal static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        #region Geometry

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
                angle += angleSign * Vector2.CalculateAngle(a, b, c);

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

        #endregion

        #region circles
        public static Vector2 PointInCircleEdge(float percent)
        {
            var angle = percent * Math.PI * 2;
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        #endregion

        #region random

        public static Vector2 RandomPointInCircleEdge()
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }
        public static Vector2 RandomPointInsideCircle()
        {
            var angle = Random.Shared.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(Random.Shared.NextDouble());
            var x = radius * Math.Cos(angle);
            var y = radius * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Returns if a value if zero withing a margin of error. (epsilon)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static bool IsAlmostZero(float value)
        {
            if (value < float.Epsilon && value > -float.Epsilon)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}