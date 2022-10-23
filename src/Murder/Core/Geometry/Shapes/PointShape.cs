namespace Murder.Core.Geometry
{
    internal readonly struct PointShape : IShape
    {
        public readonly Point Point = Point.Zero;

        public PointShape(Point point)
        {
            Point = point;
        }

        public Rectangle GetBoundingBox() => new Rectangle(Point, Point.One);
    }
}
