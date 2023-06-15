using Murder.Components;
using Murder.Utilities;
using System.Reflection.Metadata;

namespace Murder.Core.Geometry
{
    public struct Vector2 : IEquatable<Vector2>
    {
        private static readonly Vector2 _zero = new();
        private static readonly Vector2 _one = new(1,1);
        private static readonly Vector2 _center = new(0.5f, 0.5f);
        
        private static readonly Vector2 _up = new(0, -1f);
        private static readonly Vector2 _down= new(0, 1f);
        private static readonly Vector2 _left = new(-1, 0);
        private static readonly Vector2 _right = new(1, 0f);

        public static Vector2 Zero => _zero;
        public static Vector2 One => _one;
        public static Vector2 Center => _center;
        public static Vector2 Up => _up;
        public static Vector2 Down => _down;
        public static Vector2 Left => _left;
        public static Vector2 Right => _right;
        
        /// <summary>
        /// A quick shorthand for when using a vector as a "size"
        /// </summary>
        public float Width => X;

        /// <summary>
        /// A quick shorthand for when using a vector as a "size"
        /// </summary>
        public float Height => Y;

        public Vector2 Absolute => new(Math.Abs(X), MathF.Abs(Y));

        public float X;
        public float Y;

        public Vector2(float x, float y) => (X, Y) = (x, y);
        public Vector2(float v) => (X, Y) = (v, v);

        public static implicit operator System.Numerics.Vector2(Vector2 v) => new(v.X, v.Y);
        public static implicit operator Microsoft.Xna.Framework.Vector2(Vector2 v) => new(v.X, v.Y);
        public static implicit operator Vector2(Microsoft.Xna.Framework.Vector2 v) => new(v.X, v.Y);
        public static explicit operator Vector2(string str) => Parse(str);

        public static implicit operator Vector2(System.Numerics.Vector2 v) => new(v.X, v.Y);

        public static bool operator ==(Vector2 a, Microsoft.Xna.Framework.Vector2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2 a, Microsoft.Xna.Framework.Vector2 b) => a.X != b.X || a.Y != b.Y;
        public static Vector2 operator +(Vector2 a, Microsoft.Xna.Framework.Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator +(Microsoft.Xna.Framework.Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Microsoft.Xna.Framework.Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator -(Microsoft.Xna.Framework.Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator -(Vector2 vector) => new(-vector.X, -vector.Y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);
        public static Vector2 operator *(Microsoft.Xna.Framework.Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(Vector2 a, Microsoft.Xna.Framework.Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(Vector2 a, System.Numerics.Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(System.Numerics.Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(Point a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator *(Vector2 a, Point b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        
        public bool HasValue => X != 0 || Y != 0;

        public static Vector2 operator *(float b, Vector2 a) => new(a.X * b, a.Y * b);
        public static Vector2 operator *(Vector2 a, float b) => new(a.X * b, a.Y * b);
        public static Vector2 operator +(Vector2 a, float b) => new(a.X + b, a.Y + b);

        public static Vector2 operator /(Vector2 a, float b) => new(a.X / b, a.Y / b);

        public static Vector2 operator -(Point l, Vector2 r) => new(l.X - r.X, l.Y - r.Y);
        public static Vector2 operator -(Vector2 l, Point r) => new(l.X - r.X, l.Y - r.Y);

        public static Vector2 operator +(Point l, Vector2 r) => new(l.X + r.X, l.Y + r.Y);
        public static Vector2 operator +(Vector2 l, Point r) => new(l.X + r.X, l.Y + r.Y);
        
        public PositionComponent ToPosition() => new(X, Y);
        public Microsoft.Xna.Framework.Vector3 ToVector3() => new(X, Y, 0);

        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max) =>
            new(Math.Clamp(value1.X, min.X, max.X), Math.Clamp(value1.Y, min.Y, max.Y));

        public float Manhattan() => MathF.Abs(X) + MathF.Abs(Y);
        /// <summary>
        /// Cheaper than checking <see cref="Length"/>, useful when comparing distances.
        /// </summary>
        /// <returns></returns>
        public float LengthSquared() => X * X + Y * Y;
        public float Length() => MathF.Sqrt(LengthSquared());
        public Vector2 Normalized()
        {
            float distance = Length();
            return new Vector2(X / distance, Y / distance);
        }
        public static Vector2 Round(Vector2 vector) => new Vector2(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));

        public Point Point => new(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));

        public (float x, float y) XY => (X, Y);

        public Vector2 Perpendicular => new(Y, -X);

        public Point Round() => new(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));
        public Point Floor() => new(Calculator.FloorToInt(X), Calculator.FloorToInt(Y));
        public Point Ceil() => new(Calculator.CeilToInt(Math.Abs(X)) * Math.Sign(X), Calculator.CeilToInt(Math.Abs(Y)) * Math.Sign(Y));

        public Vector2 Reverse() => new Vector2(-X, -Y);

        public float Dot(Vector2 other) => X * other.X + Y * other.Y;
        public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;
        

        public Point ToGridPoint() =>
            new(
                Calculator.FloorToInt(X / Grid.CellSize),
                Calculator.FloorToInt(Y / Grid.CellSize));

        public static Vector2 Lerp(Vector2 origin, Vector2 target, float factor) => new(Calculator.Lerp(origin.X, target.X, factor), Calculator.Lerp(origin.Y, target.Y, factor));
        public static Vector2 LerpSnap(Vector2 origin, Vector2 target, float factor, float threshold = 0.01f) => 
            new(Calculator.LerpSnap(origin.X, target.X, factor, threshold), Calculator.LerpSnap(origin.Y, target.Y, factor, threshold));
        
        public static Vector2 LerpSnap(Vector2 origin, Vector2 target, double factor, float threshold = 0.01f) =>
            new((float)Calculator.LerpSnap(origin.X, target.X, factor, threshold), (float)Calculator.LerpSnap(origin.Y, target.Y, factor, threshold));

        public System.Numerics.Vector2 ToSys() => new(X,Y);
        public Microsoft.Xna.Framework.Vector2 ToXna() => new(X,Y);
        public override string ToString() => $"{X},{Y}";
        public static Vector2 Parse(String str)
        {
            var split = str.Split(',');
            return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
        }

        public bool Equals(Vector2 other) => other.X == X && other.Y == Y;

        public override bool Equals(object? obj) => obj is Vector2 p && this.Equals(p);

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static float Distance(Vector2 a, Vector2 b) => (a - b).Length();

        public float Angle()
        {
            return MathF.Atan2(Y, X);
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

        /// <summary>
        /// Returns a new vector, rotated by the given angle. In radians.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2 Rotate(float angle)
        {
            if (angle == 0) return this;
            return new Vector2(
                (float)(X * Math.Cos(angle) - Y * Math.Sin(angle)),
                (float)(X * Math.Sin(angle) + Y * Math.Cos(angle))
            );
        }

        internal float Cross(Vector2 s)
        {
            return X * s.Y - Y * s.X;
        }
        public Vector2 Mirror(Vector2 center) => new(center.X - (X - center.X), Y);

    }
}
