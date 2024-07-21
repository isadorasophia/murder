using Murder.Utilities;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public record struct LazyShape : IShape
    {
        public readonly float Radius;
        public readonly Point Offset;
        public const float SQUARE_ROOT_OF_TWO = 1.41421356237f;

        public Rectangle Rectangle(Vector2 addPosition)
        {
            return new(addPosition.X + Offset.X - Radius * 1.25f, addPosition.Y + Offset.Y - Radius, Radius * 2.5f, Radius * 2);
        }

        public Rectangle GetBoundingBox()
        {
            return new(Offset.X - Radius * 1.25f, Offset.Y - Radius, Radius * 2.5f, Radius * 2);
        }

        public LazyShape(float radius, Point offset)
        {
            Radius = radius;
            Offset = offset;
        }

        internal bool Touches(Circle circle, Point offset1, Point offset2)
        {
            var center1 = offset1 + new Point(Calculator.RoundToInt(circle.X), Calculator.RoundToInt(circle.Y));
            var center2 = offset2 + Offset;

            return (center1 - center2).LengthSquared() <= MathF.Pow(Radius + circle.Radius, 2);
        }

        internal bool Touches(LazyShape lazy2, Point offset1, Point offset2)
        {
            var center1 = offset1 + Offset;
            var center2 = offset2 + lazy2.Offset;

            return (center1 - center2).Length() <= Radius + lazy2.Radius;
        }

        internal bool Touches(Point point)
        {
            var delta = Offset - point;
            return delta.LengthSquared() <= MathF.Pow(Radius, 2);
        }

        private PolygonShape? _polygonCache = null;
        public PolygonShape GetPolygon()
        {
            _polygonCache ??= new PolygonShape(
                new Polygon(
                        new Vector2[] {
                            new(Offset.X, Offset.Y - Radius),
                            new(Offset.X + Radius * 0.75f, Offset.Y - Radius * 0.75f),
                            new(Offset.X + Radius * 1.25f, Offset.Y),
                            new(Offset.X + Radius* 0.75f, Offset.Y + Radius * 0.75f),
                            new(Offset.X, Offset.Y + Radius),
                            new(Offset.X - Radius* 0.75f, Offset.Y + Radius * 0.75f),
                            new(Offset.X - Radius * 1.25f, Offset.Y),
                            new(Offset.X - Radius * 0.75f, Offset.Y - Radius * 0.75f),
                        }
                    )
                );
            return _polygonCache.Value;
        }
    }
}