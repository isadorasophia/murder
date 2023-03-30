namespace Murder.Core.Geometry
{
    public struct PointShape : IShape
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
                new Polygon(
                        new Point[] {
                            new Point(Point.X, Point.Y - 1),
                            new Point(Point.X - 1, Point.Y),
                            new Point(Point.X + 1, Point.Y)
                        }
                    )
                );
            return _polygonCache.Value;
        }
    }
}
