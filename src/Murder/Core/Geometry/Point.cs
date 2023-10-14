using Murder.Components;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public struct Point : IEquatable<Point>
    {
        private static readonly Point _zero = new();
        private static readonly Point _down = new(0, 1);
        private static readonly Point _one = new(1, 1);
        private static readonly Point _flipped = new(-1, 1);

        public static Point One => _one;
        public static Point Flipped => _flipped;
        public static Point Zero => _zero;
        public static Point Down => _down;
        public (int x, int y) XY => (X, Y);

        public static Point HalfCell => new(Grid.CellSize / 2 + Grid.CellSize / 2);

        public int X;
        public int Y;

        public (int px, int py) BreakInTwo() => (X, Y);

        public static implicit operator Vector2(Point p) => new(p.X, p.Y);
        public static implicit operator Microsoft.Xna.Framework.Point(Point p) => new(p.X, p.Y);
        public static implicit operator Microsoft.Xna.Framework.Vector2(Point p) => new(p.X, p.Y);

        public static implicit operator Point(Microsoft.Xna.Framework.Point p) => new(p.X, p.Y);
        public static explicit operator Point(string p)
        {
            var split = p.Trim('(', ')').Split(',');
            return new Point(int.Parse(split[0]), int.Parse(split[1]));
        }

        public Point ToWorldPosition() => new(x: X * Grid.CellSize, y: Y * Grid.CellSize);

        public Point(int x, int y) => (X, Y) = (x, y);

        public Point(float x, float y) => (X, Y) = (Calculator.RoundToInt(x), Calculator.RoundToInt(y));

        public Point(int v) => (X, Y) = (v, v);

        public static bool operator ==(Point l, Point r) => l.Equals(r);

        public static bool operator !=(Point l, Point r) => !(l == r);

        public static Point operator *(Point l, float r) => new(Calculator.RoundToInt(l.X * r), Calculator.RoundToInt(l.Y * r));
        public static Point operator *(Point l, int r) => new(l.X * r, l.Y * r);
        public static Point operator *(float r, Point l) => new(Calculator.RoundToInt(l.X * r), Calculator.RoundToInt(l.Y * r));
        public static Point operator *(int r, Point l) => new(l.X * r, l.Y * r);
        public static Point operator /(Point l, float r) => new(Calculator.RoundToInt(l.X / r), Calculator.RoundToInt(l.Y / r));
        public static Point operator *(Point l, Point r) => new(l.X * r.X, l.Y * r.Y);
        public static Point operator +(Point l, Point r) => new(l.X + r.X, l.Y + r.Y);
        public static Point operator -(Point l, Point r) => new(l.X - r.X, l.Y - r.Y);
        public static Point operator -(Point l, Microsoft.Xna.Framework.Point r) => new(l.X - r.X, l.Y - r.Y);
        public static Point operator -(Microsoft.Xna.Framework.Point l, Point r) => new(l.X - r.X, l.Y - r.Y);
        public static Point operator -(Point p) => new(-p.X, -p.Y);

        public static Vector2 operator +(IMurderTransformComponent a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(IMurderTransformComponent a, Point b) => new(a.X - b.X, a.Y - b.Y);

        public bool Equals(Point other) => other.X == X && other.Y == Y;

        public override bool Equals(object? obj) => obj is Point p && this.Equals(p);

        public override int GetHashCode() => (X, Y).GetHashCode();
        public Vector2 ToVector2() => new(X, Y);
        public override string ToString() => (X, Y).ToString();

        public int LengthSquared() => X * X + Y * Y;
        public float Length() => MathF.Sqrt(LengthSquared());

        public Point Mirror(Point center) => new(center.X - (X - center.X), Y);

        public Point Scale(Point other) => new Point(X * other.X, Y * other.Y);
        public Microsoft.Xna.Framework.Vector3 ToVector3() => new(X, Y, 0);

        internal Point Clamp(int minX, int minY, int maxX, int maxY) => new(Math.Clamp(X, minX, maxX), Math.Clamp(Y, minY, maxY));

    }
}