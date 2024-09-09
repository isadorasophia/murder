using Murder.Core.Geometry;
using System.Net.Http.Headers;
using System.Numerics;

namespace Murder.Utilities
{
    public static class Vector2Helper
    {
        public static Vector2 Center { get; } = new(0.5f, 0.5f);
        public static Vector2 Down { get; } = new(0, 1);
        public static Vector2 Up { get; } = new(0, -1);
        public static Vector2 Right { get; } = new(1,0);
        public static Vector2 Left { get; } = new(-1, 0);
        public static Vector2 Max(this Vector2 first, Vector2 second) => new Vector2(Math.Max(first.X, second.X), Math.Max(first.Y, second.Y));
        public static Vector2 Min(this Vector2 first, Vector2 second) => new Vector2(Math.Min(first.X, second.X), Math.Min(first.X, second.Y));
        public static Vector2 LerpSmooth(Vector2 from, Vector2 to, float deltaTime, float halfLife) =>
            new Vector2(Calculator.LerpSmooth(from.X, to.X, deltaTime, halfLife), Calculator.LerpSmooth(from.Y, to.Y, deltaTime, halfLife));

        public static Vector2 LerpSnap(Vector2 origin, Vector2 target, float factor, float threshold = 0.01f) =>
            new(Calculator.LerpSnap(origin.X, target.X, factor, threshold),
                Calculator.LerpSnap(origin.Y, target.Y, factor, threshold));

        public static Vector2 LerpSnap(Vector2 origin, Vector2 target, double factor, float threshold = 0.01f) =>
            new((float)Calculator.LerpSnap(origin.X, target.X, factor, threshold),
                (float)Calculator.LerpSnap(origin.Y, target.Y, factor, threshold));

        public static Vector2 Project (Vector2 vector, Vector2 onNormal)
        {
            float dot = Vector2.Dot(vector, onNormal);
            float sqr = onNormal.LengthSquared();
            return onNormal * (dot / sqr);
        }

        public static Vector2 Rejection(Vector2 vector, Vector2 onNormal)
        {
            return vector - Project(vector, onNormal);
        }

        public static Vector2 RoundTowards(Vector2 value, Vector2 towards)
        {
            float roundX = value.X < towards.X ? MathF.Ceiling(value.X) : MathF.Floor(value.X);
            float roundY = value.Y < towards.Y ? MathF.Ceiling(value.Y) : MathF.Floor(value.Y);

            return new Vector2(roundX, roundY);
        }

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

        public static float Deviation(Vector2 vec1, Vector2 vec2)
        {
            // Calculate the dot product
            float dotProduct = Vector2.Dot(vec1.Normalized(), vec2.Normalized());

            // Cosine values range from -1 to 1, mapping it to 0-1
            float deviation = (dotProduct + 1) / 2;

            return 1 - deviation;
        }

        /// <summary>
        /// Returns a one unit vector, squished by <paramref name="ammount"/>.
        /// A positive number increases the X, a negative number increases the Y
        /// </summary>
        /// <param name="ammount"></param>
        public static Vector2 Squish(float ammount)
        {
            return new Vector2(1 + ammount, 1 - ammount);
        }


        /// <summary>
        /// Returns the inner vector scaled to fit inside the outter vector, keeping the aspect ratio.
        /// </summary>
        /// <param name="outter"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        internal static Point FitInside(Vector2 outter, Vector2 inner)
        {
            float ratio = Math.Min(outter.X / inner.X, outter.Y / inner.Y);
            return new Point((int)(inner.X * ratio), (int)(inner.Y * ratio));
        }
    }
}