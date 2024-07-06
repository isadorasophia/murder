using Murder.Core;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Utilities
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// A quick shorthand for when using a vector as a "size"
        /// </summary>
        public static float Width(this Vector2 vector) => vector.X;

        /// <summary>
        /// A quick shorthand for when using a vector as a "size"
        /// </summary>
        public static float Height(this Vector2 vector) => vector.Y;

        public static bool HasValue(this Vector2 vector) => vector.X != 0 || vector.Y != 0;

        public static Vector2 Add(this Vector2 a, float b) => new(a.X + b, a.Y + b);
        public static Vector2 Multiply(this Vector2 a, Microsoft.Xna.Framework.Vector2 b) => new(a.X * b.X, a.Y * b.Y);

        public static Microsoft.Xna.Framework.Vector3 ToVector3(this Vector2 vector) => new(vector.X, vector.Y, 0);

        public static float Manhattan(this Vector2 vector) => MathF.Abs(vector.X) + MathF.Abs(vector.Y);

        public static Vector2 Normalized(this Vector2 vector)
        {
            float distance = vector.Length();
            return new Vector2(vector.X / distance, vector.Y / distance);
        }
        public static Vector2 NormalizedWithSanity(this Vector2 vector)
        {
            float distance = vector.Length();
            if (distance == 0)
                return Vector2.Zero;
            return new Vector2(vector.X / distance, vector.Y / distance);
        }

        public static Point Point(this Vector2 vector) =>
            new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));

        public static (float x, float y) XY(this Vector2 vector) => (vector.X, vector.Y);

        public static Vector2 Abs(this Vector2 vector) => new(MathF.Abs(vector.X), MathF.Abs(vector.Y));
        public static Point Ceiling(this Vector2 vector) =>
            new(Calculator.CeilToInt(vector.X), Calculator.CeilToInt(vector.Y));
        public static Point Round(this Vector2 vector) =>
            new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));
        public static Point Floor(this Vector2 vector) =>
            new(Calculator.FloorToInt(vector.X), Calculator.FloorToInt(vector.Y));

        public static Vector2 Reverse(this Vector2 vector) => new(-vector.X, -vector.Y);

        public static Point ToGridPoint(this Vector2 vector) =>
            new(
                Calculator.FloorToInt(vector.X / Grid.CellSize),
                Calculator.FloorToInt(vector.Y / Grid.CellSize));

        /// <summary>
        /// Returns a new vector, rotated by the given angle. In radians.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="angle">The angle to rotate by.</param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            if (angle == 0) return vector;
            return new Vector2(
                (float)(vector.X * Math.Cos(angle) - vector.Y * Math.Sin(angle)),
                (float)(vector.X * Math.Sin(angle) + vector.Y * Math.Cos(angle))
            );
        }

        public static Vector2 Mirror(this Vector2 vector, Vector2 center) =>
            new(center.X - (vector.X - center.X), vector.Y);


        public static Vector2 ClampLength(this Vector2 vector, float length)
        {
            return vector.LengthSquared() > length * length ? vector.Normalized() * length : vector;
        }

        public static Vector2 PerpendicularClockwise(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static float PerpendicularCounterClockwise(this Vector2 vector, Vector2 other)
        {
            // Calculate the dot product
            float dotProduct = Vector2.Dot(vector.Normalized(), other.Normalized());

            // Cosine values range from -1 to 1, mapping it to 0-1
            float deviation = (dotProduct + 1) / 2;

            return 1 - deviation;
        }

        public static Vector2 Approach(this Vector2 a, Vector2 b, float amount)
        {
            return new Vector2(Calculator.Approach(a.X, b.X, amount), Calculator.Approach(b.Y, a.Y, amount));
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        /// <summary>
        /// Snap the angel to the nearest angle, based on the number of steps in a circle.
        /// </summary>
        public static Vector2 SnapAngle(this Vector2 vector, int steps)
        {
            float angle = vector.Angle();
            float step = MathF.PI * 2 / steps;
            float snapped = MathF.Round(angle / step) * step;
            return new Vector2(MathF.Cos(snapped), MathF.Sin(snapped));
        }

        /// <summary>
        /// Returns the perpendicular vector to the given vector.
        /// </summary>
        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }
    }
}