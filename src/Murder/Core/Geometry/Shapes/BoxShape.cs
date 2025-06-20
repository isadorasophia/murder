using Murder.Attributes;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public record struct BoxShape : IShape
    {
        public readonly int Width = 16;
        public readonly int Height = 16;

        [Slider]
        public readonly Vector2 Origin = Vector2.Zero;
        public readonly Point Offset { get; init; } = Point.Zero;

        public Point Size => new(Width, Height);

        public BoxShape() { }

        /// <summary>
        /// Simple shape getter
        /// </summary>
        public Rectangle Rectangle => new Rectangle(-Calculator.RoundToInt(Width * Origin.X) + Offset.X, -Calculator.RoundToInt(Height * Origin.Y) + Offset.Y, Width, Height);


        public Rectangle GetBoundingBox() => new(Offset - Origin * Size, Size);

        public BoxShape(Rectangle rectangle)
        {
            Origin = Vector2.Zero;
            Offset = rectangle.TopLeft.Point();
            Width = (int)rectangle.Width;
            Height = (int)rectangle.Height;
        }

        public BoxShape(Vector2 origin, Point offset, int width, int height)
        {
            Origin = origin;
            Offset = offset;
            Width = width;
            Height = height;
        }

        public BoxShape ResizeTopLeft(Vector2 newTopLeft)
        {
            Vector2 delta = Offset - newTopLeft;
            return new BoxShape(
                Origin,
                newTopLeft.Point(),
                Width + Calculator.RoundToInt(delta.X),
                Height + Calculator.RoundToInt(delta.Y)
                );
        }
        public BoxShape ResizeBottomRight(Vector2 newBottomRight)
        {
            Point origin = ((Vector2.One - Origin) * Size).Point();
            Vector2 delta = Offset + origin - newBottomRight;
            return new BoxShape(
                Origin,
                Offset,
                Width - (int)delta.X,
                Height - (int)delta.Y
                );
        }

        private PolygonShape? _polygonCache = null;
        public PolygonShape GetPolygon()
        {
            _polygonCache ??= new PolygonShape(
                new Polygon(
                [
                    Rectangle.TopLeft,
                    Rectangle.TopRight,
                    Rectangle.BottomRight,
                    Rectangle.BottomLeft
                ])
            );

            return _polygonCache.Value;
        }
    }
}