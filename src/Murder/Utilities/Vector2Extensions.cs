using System.Numerics;

namespace Murder.Utilities
{
    public static class Vector2Extensions
    {
        public static bool HasValue(this Vector2 vector) => vector.X != 0 || vector.Y != 0;

        public static Vector2 Normalized(this Vector2 vector)
        {
            float distance = vector.Length();
            return new Vector2(vector.X / distance, vector.Y / distance);
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

        public static Vector2 PerpendicularClockwise(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector) {
            return new Vector2(-vector.Y, vector.X);
        }
    }
}

