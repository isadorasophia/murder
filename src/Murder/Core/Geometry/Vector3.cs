using Murder.Components;
using Murder.Utilities;

namespace Murder.Core.Geometry
{
    public struct Vector3 : IEquatable<Vector3>
    {
        private static readonly Vector3 _zero = new();
        private static readonly Vector3 _one = new(1,1,1);
        private static readonly Vector3 _center = new(0.5f, 0.5f, 0.5f);
        
        public static Vector3 Zero => _zero;
        public static Vector3 One => _one;
        public static Vector3 Center => _center;
        
        public Vector3 Absolute => new(Math.Abs(X), MathF.Abs(Y), MathF.Abs(Z));

        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z) => (X, Y, Z) = (x, y, z);

        public static implicit operator System.Numerics.Vector3(Vector3 v) => new(v.X, v.Y, v.Z);
        public static implicit operator Microsoft.Xna.Framework.Vector3(Vector3 v) => new(v.X, v.Y, v.Z);
        public static implicit operator Vector3(Microsoft.Xna.Framework.Vector3 v) => new(v.X, v.Y, v.Z);
        public static explicit operator Vector3(string str) => Parse(str);

        public static implicit operator Vector3(System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);

        public static Vector3 operator -(Vector3 vector) => new(-vector.X, -vector.Y, -vector.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        internal bool HasValue => X != 0 || Y != 0 || Z != 0;

        public PositionComponent ToPosition() => new(X, Y);
        public Microsoft.Xna.Framework.Vector3 ToVector3() => new(X, Y, Z);

        public static Vector3 Round(Vector3 vector) => 
            new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y), Calculator.RoundToInt(vector.Z));

        public float Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;
        public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        
        public static Vector3 Lerp(Vector3 origin, Vector3 target, float factor) => 
            new(Calculator.Lerp(origin.X, target.X, factor), Calculator.Lerp(origin.Y, target.Y, factor), Calculator.Lerp(origin.Z, target.Z, factor));
        
        public System.Numerics.Vector3 ToSys() => new(X, Y, Z);
        public Microsoft.Xna.Framework.Vector3 ToXna() => new(X, Y, Z);
        public override string ToString() => $"{X},{Y},{Z}";

        public static Vector3 Parse(string str)
        {
            var split = str.Split(',');
            if (split.Length < 3)
            {
                throw new InvalidOperationException($"Unable to parse string '{str}' to Vector3.");
            }

            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }

        public bool Equals(Vector3 other) => other.X == X && other.Y == Y && other.Z == Z;

        public override bool Equals(object? obj) => obj is Vector2 p && this.Equals(p);

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        public float LengthSquared() => X * X + Y * Y + Z * Z;
        public float Length() => MathF.Sqrt(LengthSquared());

        public static float Distance(Vector3 a, Vector3 b) => (a - b).Length();

        internal float Angle()
        {
            return MathF.Atan2(Y, X);
        }
    }
}
