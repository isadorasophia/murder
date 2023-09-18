using Murder.Core;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Utilities
{
    public static class Vector2Extensions
    {
        public static bool HasValue(this Vector2 vector) => vector.X != 0 || vector.Y != 0;

        public static Vector2 Add(this Vector2 a, float b) => new(a.X + b, a.Y + b);

        public static float Manhattan(this Vector2 vector) => MathF.Abs(vector.X) + MathF.Abs(vector.Y);

        public static Vector2 Normalized(this Vector2 vector)
        {
            float distance = vector.Length();
            return new Vector2(vector.X / distance, vector.Y / distance);
        }

        public static Point Point(this Vector2 vector) =>
            new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));

        public static (float x, float y) XY(this Vector2 vector) => (vector.X, vector.Y);

        public static Point Round(this Vector2 vector) =>
            new (Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));

        public static Point ToGridPoint(this Vector2 vector) =>
            new(
                Calculator.FloorToInt(vector.X / Grid.CellSize),
                Calculator.FloorToInt(vector.Y / Grid.CellSize));

        ///<summary>
        /// Calculates the internal angle of a triangle.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float CalculateAngle(Vector2 a, Vector2 b, Vector2 c)
        {
            // Calculate the vectors AB and AC.
            Vector2 v1 = b - a;
            Vector2 v2 = c - a;

            // Calculate the dot product of the vectors.
            float dot = Vector2.Dot(v1, v2);

            // Calculate the cross product of the vectors.
            float cross = v1.X * v2.Y - v1.Y * v2.X;

            // Return the angle in radians.
            return (float)Math.Atan2(cross, dot);
        }

        /// <summary>
        /// Creates a vector from an angle in radians.
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns></returns>
        public static Vector2 FromAngle(float angle)
        {
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        /// <summary>
        /// Returns a new vector, rotated by the given angle. In radians.
        /// </summary>
        /// <param name="angle"></param>
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

        public static Vector2 PerpendicularClockwise(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }
    }
}

