using Murder.Utilities;
using Newtonsoft.Json;

namespace Murder.Core.Geometry
{
    public struct IntRectangle : IEquatable<Rectangle>
    {
        public static IntRectangle Empty => _empty;
        public static IntRectangle One => _one;
        private static IntRectangle _empty = new();
        private static IntRectangle _one = new(0, 0, 1, 1);

        public int X;
        public int Y;
        public int Width;
        public int Height;

        // Quick Helpers
        public Point TopLeft => new Point(X, Y);
        public Point TopRight => new Point(X + Width, Y);
        public Point BottomRight => new Point(X + Width, Y + Height);
        public Point BottomLeft => new Point(X, Y + Height);
        public Point CenterPoint => new(X + Calculator.RoundToInt(Width / 2f), Y + Calculator.RoundToInt(Height / 2f));
        public Vector2 Center => new(X + (Width / 2f), Y + (Height / 2f));
        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;
        
        [JsonIgnore]
        public Point Size
        {
            get
            {
                return new(Width, Height);

            }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        // Implicit conversions
        public static implicit operator IntRectangle(Rectangle p) => new(p.X, p.Y, p.Width, p.Height);
        public static implicit operator IntRectangle(Microsoft.Xna.Framework.Rectangle p) => new(p.X, p.Y, p.Width, p.Height);
        public static implicit operator Microsoft.Xna.Framework.Rectangle(IntRectangle p) => new(p.X, p.Y, p.Width, p.Height);

        public static Rectangle operator +(IntRectangle p, Point v) => new(p.X + v.X, p.Y + v.Y, p.Width, p.Height);
        public static IntRectangle operator *(IntRectangle p, float v) => new(p.X * v, p.Y * v, p.Width * v, p.Height * v);
        public static IntRectangle operator /(IntRectangle p, float v) => new(p.X / v, p.Y / v, p.Width / v, p.Height / v);

        public IntRectangle AddPosition(Vector2 position) => new IntRectangle(X + Calculator.RoundToInt(position.X), Y + Calculator.RoundToInt(position.Y), Width, Height);
        public IntRectangle AddPosition(Point position) => new IntRectangle(X + position.X, Y + position.Y, Width, Height);
        public IntRectangle Expand(int value) => new IntRectangle(X - value, Y - value, Width + value * 2, Height + value * 2);
        public IntRectangle Expand(float value) => new IntRectangle(X - value, Y - value, Width + value * 2, Height + value * 2);

        public IntRectangle(Point position, Point size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }
        public IntRectangle(float x, float y, float width, float height)
        {
            X = Calculator.RoundToInt(x);
            Y = Calculator.RoundToInt(y);
            Width = Calculator.RoundToInt(width);
            Height = Calculator.RoundToInt(height);
        }

        public IntRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public IntRectangle(Point p, int width, int height) : this(p.X, p.Y, width, height) { }

        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Gets whether or not the other <see cref="Rectangle"/> intersects with this rectangle.
        /// </summary>
        /// <param name="other">The other rectangle for testing.</param>
        /// <returns><c>true</c> if other <see cref="Rectangle"/> intersects with this rectangle; <c>false</c> otherwise.</returns>
        public bool Touches(Rectangle other)
        {
            return other.Left <= Right &&
                   Left <= other.Right &&
                   other.Top <= Bottom &&
                   Top <= other.Bottom;
        }

        public bool Contains(Point point) => Contains(point.X, point.Y);
        public bool Contains(float X, float Y) => Contains(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));
        public bool Contains(int X, int Y)
        {
            return X >= Left && X < Right && Y >= Top && Y < Bottom;
        }

        public bool Contains(Rectangle other)
        {
            return Left <= other.Left &&
                   Right >= other.Right &&
                   Top <= other.Top &&
                   Bottom >= other.Bottom;
        }

        public Point ClampPosition(IntRectangle innerRect)
        {
            int clampedX = Math.Max(X, Math.Min(innerRect.X, X + Width - innerRect.Width));
            int clampedY = Math.Max(Y, Math.Min(innerRect.Y, Y + Height - innerRect.Height));

            return new(clampedX, clampedY);
        }

        public static IntRectangle CenterRectangle(Vector2 center, float width, float heigth) => new(center.X - width / 2f, center.Y - heigth / 2f, width, heigth);

    }
}
