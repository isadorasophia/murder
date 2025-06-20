using System.Numerics;

namespace Murder.Core.Geometry
{
    public record struct LineShape : IShape
    {
        public readonly Point Start = Point.Zero;
        public readonly Point End = Point.Zero;

        public LineShape(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Line2 Line => new Line2(Start, End);

        public Line2 LineAtPosition(Point position) => new Line2((Start + position).ToVector2(), (End + position).ToVector2());

        public Rectangle GetBoundingBox()
        {
            int left = Math.Min(Start.X, End.X);
            int right = Math.Max(Start.X, End.X);
            int top = Math.Min(Start.Y, End.Y);
            int bottom = Math.Max(Start.Y, End.Y);

            return new(left, top, right - left, bottom - top);
        }

        private PolygonShape? _polygonCache = null;
        public PolygonShape GetPolygon()
        {
            _polygonCache ??= new PolygonShape(
                new Polygon(
                        [
                            Line.Start,
                            Line.End
                        ]
                    )
                );

            return _polygonCache.Value;
        }
    }
}