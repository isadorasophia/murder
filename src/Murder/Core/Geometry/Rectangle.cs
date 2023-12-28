using Murder.Utilities;
using Newtonsoft.Json;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static Rectangle Empty => _empty;
        public static Rectangle One => _one;
        private static Rectangle _empty = new();
        private static Rectangle _one = new(0, 0, 1, 1);

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public int XRound => Calculator.RoundToInt(X);
        public int YRound => Calculator.RoundToInt(Y);
        public int WidthRound => Calculator.RoundToInt(Width);
        public int HeightRound => Calculator.RoundToInt(Height);

        // Quick Helpers
        public Vector2 TopLeft => new(X, Y);
        public Vector2 TopCenter => new(X + (Width / 2f), Y);
        public Vector2 TopRight => new(X + Width, Y);
        public Vector2 BottomRight => new(X + Width, Y + Height);
        public Vector2 BottomCenter => new(X + (Width / 2f), Y + Height);
        public Vector2 BottomLeft => new(X, Y + Height);
        public Vector2 CenterLeft => new(X, Y + (Height / 2f));
        public Point CenterPoint => new(X + Calculator.RoundToInt(Width / 2f), Y + Calculator.RoundToInt(Height / 2f));
        public Vector2 Center => new(X + (Width / 2f), Y + (Height / 2f));

        [JsonIgnore]
        public float Left { get => X; set => X = value; }

        [JsonIgnore]
        public float Right { get => X + Width; set => Width = X - value; }

        [JsonIgnore]
        public float Top { get => Y; set => Y = value; }

        [JsonIgnore]
        public float Bottom { get => Y + Height; set => Height = value - Y; }

        [JsonIgnore]
        public Vector2 Size
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

        public bool IsEmpty => X == 0 && Y == 0 && Width == 0 && Height == 0;

        // Implicit conversions
        public static bool operator ==(Rectangle a, Rectangle b) => a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
        public static Rectangle operator *(Rectangle p, float v) => new(p.X * v, p.Y * v, p.Width * v, p.Height * v);
        public static Rectangle operator +(Rectangle p, Point v) => new(p.X + v.X, p.Y + v.Y, p.Width, p.Height);
        public static Rectangle operator -(Rectangle p, Point v) => new(p.X - v.X, p.Y - v.Y, p.Width, p.Height);
        public static implicit operator Rectangle(Microsoft.Xna.Framework.Rectangle p) => new(p.X, p.Y, p.Width, p.Height);
        public static implicit operator Microsoft.Xna.Framework.Rectangle(Rectangle p) => new(Calculator.RoundToInt(p.X), Calculator.RoundToInt(p.Y), Calculator.RoundToInt(p.Width), Calculator.RoundToInt(p.Height));

        /// <summary>
        /// Constructor for a rectangle from a set of coordinates.
        /// </summary>
        public static Rectangle FromCoordinates(float top, float bottom, float left, float right)
        {
            return new Rectangle(
                left,
                top,
                right - left,
                bottom - top
                );
        }

        /// <summary>
        /// Constructor for a rectangle from a set of coordinates.
        /// </summary>
        public static Rectangle FromCoordinates(Vector2 topLeft, Vector2 bottomRight) =>
            FromCoordinates(top: topLeft.Y, bottom: bottomRight.Y, left: topLeft.X, right: bottomRight.X);

        public static implicit operator Rectangle(IntRectangle p) => new(p.X, p.Y, p.Width, p.Height);

        public Rectangle AddPosition(Vector2 position) => new Rectangle(X + Calculator.RoundToInt(position.X), Y + Calculator.RoundToInt(position.Y), Width, Height);
        public Rectangle AddPosition(Point position) => new Rectangle(X + position.X, Y + position.Y, Width, Height);
        public Rectangle SetPosition(Vector2 position) => new Rectangle(position.X, position.Y, Width, Height);
        public Rectangle Expand(int value) => new Rectangle(X - value, Y - value, Width + value * 2, Height + value * 2);
        public Rectangle Expand(float value) => new Rectangle(X - value, Y - value, Width + value * 2, Height + value * 2);

        public Rectangle(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public Rectangle(Point position, Point size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }
        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Point p, int width, int height) : this(p.X, p.Y, width, height) { }

        public static Rectangle GetIntersection(Rectangle a, Rectangle b)
        {
            return FromAbsolute(
                Math.Max(a.Left, b.Left),
                Math.Min(a.Right, b.Right),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Bottom, b.Bottom));
        }

        private static Rectangle FromAbsolute(float left, float right, float top, float bottom)
        {
            return new(left, top, right - left, bottom - top);
        }

        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Whether an object within bounds intersects with this rectangle.
        /// This takes into account the "maximum" height and length given any rotation.
        /// </summary>
        public bool TouchesWithMaxRotationCheck(Vector2 position, Vector2 size, Vector2 offset)
        {
            float maxSizeValue = Math.Max(size.Width(), size.Height());

            Vector2 maxSize = new Vector2(maxSizeValue, maxSizeValue);
            Vector2 maxOffset = position + (size - maxSize) / 2f;

            return Touches(new Rectangle(maxOffset - size * offset, maxSize));
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

        /// <summary>
        /// Gets whether or not the other <see cref="Rectangle"/> intersects with this rectangle.
        /// </summary>
        /// <param name="other">The other rectangle for testing.</param>
        /// <returns><c>true</c> if other <see cref="Rectangle"/> intersects with this rectangle; <c>false</c> otherwise.
        /// Note that this returns <c>false</c> if they only touch, they need to be inside one another.</returns>
        public bool TouchesInside(Rectangle other)
        {
            return other.Left < Right &&
                   Left < other.Right &&
                   other.Top < Bottom &&
                   Top < other.Bottom;
        }

        public bool Contains(Vector2 vector) => Contains(vector.X, vector.Y);
        public bool Contains(float X, float Y) => Contains(Calculator.RoundToInt(X), Calculator.RoundToInt(Y));

        public bool Contains(Point point) => Contains(point.X, point.Y);
        public bool Contains(int X, int Y)
        {
            return X > Left && X < Right && Y > Top && Y < Bottom;
        }

        public Rectangle CenterRectangle(float x, float y) => new Rectangle(X + (Width - x) / 2f, Y + (Height - y) / 2f, x, y);
        public Rectangle CenterRectangle(Vector2 size)
        {
            return new Rectangle(Center - size / 2f, size);
        }

        public static Rectangle Lerp(Rectangle a, Rectangle b, float v)
        {
            return new Rectangle(
                Calculator.Lerp(a.X, b.X, v),
                Calculator.Lerp(a.Y, b.Y, v),
                Calculator.Lerp(a.Width, b.Width, v),
                Calculator.Lerp(a.Height, b.Height, v)
                );
        }

        public static Rectangle CenterRectangle(Point center, int width, int height) => new(center.X - width / 2f, center.Y - height / 2f, width, height);
        public static Rectangle CenterRectangle(Vector2 center, float width, float height) => new(center.X - width / 2f, center.Y - height / 2f, width, height);

        internal Rectangle ExpandToGrid()
        {
            float left = Grid.FloorToGrid(X) * Grid.CellSize;
            float right = Grid.CeilToGrid(X + Width) * Grid.CellSize;
            float top = Grid.FloorToGrid(Y) * Grid.CellSize;
            float bottom = Grid.CeilToGrid(Y + Height + Grid.HalfCellSize) * Grid.CellSize;
            return new Rectangle(left, top, right - left, bottom - top);
        }

        public override bool Equals(object? obj)
        {
            return obj is Rectangle other &&
                X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        public Rectangle AddPadding(float left, float top, float right, float bottom)
        {
            return new Rectangle(X - left, Y - top, Width + left + right, Height + top + bottom);
        }
    }
}