﻿using Murder.Core;
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

        public static Point Point(this Vector2 vector) =>
            new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));

        public static (float x, float y) XY(this Vector2 vector) => (vector.X, vector.Y);

        public static Point Round(this Vector2 vector) =>
            new (Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));
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

