using Murder.Attributes;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Core.Geometry
{
    public record struct CircleShape : IShape
    {
        public readonly float Radius;

        [Slider]
        public readonly Point Offset;

        public Circle Circle => new(Offset.X, Offset.Y, Radius);

        public CircleShape(float radius, Point offset) => (Radius, Offset) = (radius, offset);

        public Rectangle GetBoundingBox()
        {
            int radius = Calculator.RoundToInt(Radius);
            int diameter = Calculator.RoundToInt(Radius * 2);
            return new Rectangle(Offset.X - radius, Offset.Y - radius, diameter, diameter);
        }

        private PolygonShape? _polygonCache = null;
        public PolygonShape GetPolygon()
        {
            _polygonCache ??= new PolygonShape(
                new Polygon(Circle.MakePolygon())
                );
            return _polygonCache.Value;
        }
    }
}