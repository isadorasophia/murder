using System.Numerics;

namespace Murder.Core.Geometry
{
    public record struct PointShape : IShape
    {
        public readonly Point Point = Point.Zero;

        public PointShape(Point point)
        {
            Point = point;
        }

        public Rectangle GetBoundingBox() => new Rectangle(Point, Point.One);

        private PolygonShape? _polygonCache = null;

        public PolygonShape GetPolygon()
        {
            _polygonCache ??= new PolygonShape(
                new Polygon([
                    new Vector2(Point.X, Point.Y - 1),
                    new Vector2(Point.X - 1, Point.Y),
                    new Vector2(Point.X + 1, Point.Y)])
                );
            return _polygonCache.Value;
        }
    }
}