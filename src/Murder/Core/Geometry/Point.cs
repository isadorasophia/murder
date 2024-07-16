using Murder.Components;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core.Geometry
{
    /// <summary>
    /// Represents a single point with coordinates <see cref="X"/> and <see cref="Y"/>.
    /// Points are also often used to store sizes, with X marking the right of an object and Y marking its bottom. 
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// Point with coordinates X = 1 and Y = 1.
        /// </summary>
        public static Point One { get; } = new(1, 1);

        /// <summary>
        /// Point with coordinates X = -1 and Y = 1 (multiply your point by this and you get its mirror).
        /// </summary>
        public static Point Flipped { get; } = new(-1, 1);

        /// <summary>
        /// Point with coordinates X = 0 and Y = 0.
        /// </summary>
        public static Point Zero { get; } = new();

        /// <summary>
        /// Point with coordinates X = 0 and Y = 1.
        /// </summary>
        public static Point Down { get; } = new(0, 1);

        /// <summary>
        /// Destructuring helper for obtaining a tuple from this point.
        /// </summary>
        public (int x, int y) XY => (X, Y);

        /// <summary>
        /// Represents half a cell on the current <see cref="Grid"/>.
        /// </summary>
        public static Point HalfCell => new(Grid.CellSize / 2 + Grid.CellSize / 2);

        /// <summary>
        /// The value of X in this point.
        /// </summary>
        public int X;
        
        /// <summary>
        /// The value of Y in this point.
        /// </summary>
        public int Y;
        
        /// <summary>
        /// Deconstruction helper for obtaining a tuple from this point.
        /// </summary>
        public (int px, int py) BreakInTwo() => (X, Y);

        /// <summary>
        /// Deconstruction helper for obtaining a tuple from this point.
        /// </summary>
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Converts this point into a <see cref="Vector2"/>.
        /// </summary>
        /// <returns>A vector with x and y equal to this point's x and y.</returns>
        public static implicit operator Vector2(Point p) => new(p.X, p.Y);

        /// <summary>
        /// Converts this point into a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        public static implicit operator Microsoft.Xna.Framework.Point(Point p) => new(p.X, p.Y);

        /// <summary>
        /// Converts this point into a <see cref="Microsoft.Xna.Framework.Vector2"/>.
        /// </summary>
        public static implicit operator Microsoft.Xna.Framework.Vector2(Point p) => new(p.X, p.Y);

        /// <summary>
        /// Converts a <see cref="Microsoft.Xna.Framework.Point"/> into a Murder Point.
        /// </summary>
        public static implicit operator Point(Microsoft.Xna.Framework.Point p) => new(p.X, p.Y);
        
        /// <summary>
        /// Parses a string as a point.
        /// </summary>
        public static explicit operator Point(string p)
        {
            var split = p.Trim('(', ')').Split(',');
            return new Point(int.Parse(split[0]), int.Parse(split[1]));
        }

        /// <summary>
        /// Converts this point into a world position by multiplying it by the cell size.
        /// </summary>
        public Point ToWorldPosition() => new(x: X * Grid.CellSize, y: Y * Grid.CellSize);

        /// <summary>
        /// Creates a point.
        /// </summary>
        public Point(int x, int y) => (X, Y) = (x, y);

        /// <summary>
        /// Creates a point by rounding the x and y parameters.
        /// </summary>
        public Point(float x, float y) => (X, Y) = (Calculator.RoundToInt(x), Calculator.RoundToInt(y));

        /// <summary>
        /// Creates a point where both x and y and equal to v.
        /// </summary>
        public Point(int v) => (X, Y) = (v, v);

        /// <summary>
        /// Checks whether two points have the same X and Y values.
        /// </summary>
        public static bool operator ==(Point l, Point r) => l.Equals(r);

        /// <summary>
        /// Checks whether two points have different X and Y values.
        /// </summary>
        public static bool operator !=(Point l, Point r) => !(l == r);

        /// <summary>
        /// Multiplies both the X and Y values of the point <paramref name="l"/> point by <paramref name="r"/> and creates a new point by rounding the results.
        /// </summary>
        public static Point operator *(Point l, float r) => new(Calculator.RoundToInt(l.X * r), Calculator.RoundToInt(l.Y * r));
        
        /// <summary>
        /// Multiplies both the X and Y values of the point <paramref name="l"/> by <paramref name="r"/> and creates a new point with the results.
        /// </summary>
        public static Point operator *(Point l, int r) => new(l.X * r, l.Y * r);

        /// <summary>
        /// Multiplies both the X and Y values of the point <paramref name="l"/> by <paramref name="r"/> and creates a new point by rounding the results.
        /// </summary>
        public static Point operator *(float r, Point l) => new(Calculator.RoundToInt(l.X * r), Calculator.RoundToInt(l.Y * r));
        
        /// <summary>
        /// Multiplies both the X and Y values of the point <paramref name="l"/> by <paramref name="r"/> and creates a new point with the results.
        /// </summary>
        public static Point operator *(int r, Point l) => new(l.X * r, l.Y * r);
        
        /// <summary>
        /// Divides both the X and Y values of the point <paramref name="l"/> by <paramref name="r"/> and creates a new point by rounding the results.
        /// </summary>
        public static Point operator /(Point l, float r) => new(Calculator.RoundToInt(l.X / r), Calculator.RoundToInt(l.Y / r));

        /// <summary>
        /// Multiplies both the X and Y values of the point <paramref name="l"/> by the X and Y values of the point <paramref name="r"/>.
        /// </summary>
        public static Point operator *(Point l, Point r) => new(l.X * r.X, l.Y * r.Y);
        
        /// <summary>
        /// Sums both the X and Y values of the point <paramref name="l"/> with the X and Y values of the point <paramref name="r"/>.
        /// </summary>
        public static Point operator +(Point l, Point r) => new(l.X + r.X, l.Y + r.Y);
        
        /// <summary>
        /// Subtracts both the X and Y values of the point <paramref name="l"/> by the X and Y values of the point <paramref name="r"/>.
        /// </summary>
        public static Point operator -(Point l, Point r) => new(l.X - r.X, l.Y - r.Y);
        
        /// <summary>
        /// Subtracts both the X and Y values of the point <paramref name="l"/> by the X and Y values of the point <paramref name="r"/>.
        /// </summary>
        public static Point operator -(Point l, Microsoft.Xna.Framework.Point r) => new(l.X - r.X, l.Y - r.Y);
        
        /// <summary>
        /// Subtracts both the X and Y values of the point <paramref name="l"/> by the X and Y values of the point <paramref name="r"/>.
        /// </summary>
        public static Point operator -(Microsoft.Xna.Framework.Point l, Point r) => new(l.X - r.X, l.Y - r.Y);
        
        /// <summary>
        /// The unary - operator returns a new point with -X and -Y as its coordinates.
        /// </summary>
        public static Point operator -(Point p) => new(-p.X, -p.Y);

        /// <summary>
        /// Sums both the X and Y values of the point <paramref name="b"/> with the X and Y values of the <see cref="IMurderTransformComponent"/> <paramref name="a"/>.
        /// </summary>
        public static Vector2 operator +(IMurderTransformComponent a, Point b) => new(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// Subtracts both the X and Y values of the point <paramref name="b"/> by the X and Y values of the <see cref="IMurderTransformComponent"/> <paramref name="a"/>.
        /// </summary>
        public static Vector2 operator -(IMurderTransformComponent a, Point b) => new(a.X - b.X, a.Y - b.Y);

        /// <summary>
        /// Compares whether the point <paramref name="other"/> has the same X and Y value as this point.
        /// </summary>
        public bool Equals(Point other) => other.X == X && other.Y == Y;
        
        /// <inheritdoc cref="Object"/>
        public override bool Equals(object? obj) => obj is Point p && this.Equals(p);
        
        /// <inheritdoc cref="Object"/>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Converts this point into a <see cref="Vector2"/> with the same X and Y values.
        /// </summary>
        public readonly Vector2 ToVector2() => new(X, Y);

        /// <inheritdoc cref="Object"/>
        public override string ToString() => (X, Y).ToString();

        /// <summary>
        /// Returns the length of this point, squared (IOW: X * X + Y * Y).
        /// </summary>
        public int LengthSquared() => X * X + Y * Y;

        /// <summary>
        /// Calculates the length of this point.
        /// </summary>
        /// <returns>The lenght of this point.</returns>
        public float Length() => MathF.Sqrt(LengthSquared());

        /// <summary>
        /// Returns the mirror of this point across the X axis relative to the center point <paramref name="center"/>
        /// </summary>
        /// <param name="center">The center to which the mirror will be relative to.</param>
        public Point Mirror(Point center) => new(center.X - (X - center.X), Y);

        /// <summary>
        /// Equivalent to this * other.
        /// </summary>
        /// <param name="other">The other point to multiply this by.</param>
        public Point Scale(Point other) => new Point(X * other.X, Y * other.Y);
        public Microsoft.Xna.Framework.Vector3 ToVector3() => new(X, Y, 0);

        /// <summary>
        /// Calculates a new point based on this point that is within the specified constraints.
        /// </summary>
        /// <param name="minX">The smallest possible value for X, inclusive</param>
        /// <param name="minY">The smallest possible value for Y, inclusive</param>
        /// <param name="maxX">The largest possible value for X, inclusive</param>
        /// <param name="maxY">The largest possible value for Y, inclusive</param>
        /// <returns>A new point that is guaranteed to satisfy the supplied constraints.</returns>
        internal Point Clamp(int minX, int minY, int maxX, int maxY) => new(Math.Clamp(X, minX, maxX), Math.Clamp(Y, minY, maxY));
        public Point Max(Point other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));
        public Point Min(Point other) => new(Math.Min(X, other.X), Math.Min(Y, other.Y));

        public static Point Lerp(Point point1, Point point2, float endFraction)
        {
            return new Point(Calculator.Lerp(point1.X, point2.X, endFraction), Calculator.Lerp(point1.Y, point2.Y, endFraction));
        }
        public static Point Lerp(Vector2 point1, Vector2 point2, float endFraction)
        {
            return new Point(Calculator.Lerp(point1.X, point2.X, endFraction), Calculator.Lerp(point1.Y, point2.Y, endFraction));
        }
    }
}