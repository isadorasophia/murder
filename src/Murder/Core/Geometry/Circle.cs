using Murder.Components;
using Murder.Services;

namespace Murder.Core.Geometry
{
    public readonly struct Circle
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Radius;
        internal Vector2 Center => new Vector2(X, Y);

        public Circle(float radius)
        {
            X = 0;
            Y = 0;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public Circle AddPosition(PositionComponent position) => new Circle(X + position.X, Y + position.Y, Radius);
        public Circle AddPosition(Point position) => new Circle(X + position.X, Y + position.Y, Radius);
        public Circle AddPosition(Vector2 position) => new Circle(X + position.X, Y + position.Y, Radius);

        public bool Contains(Vector2 vector2) => (new Vector2(X, Y) - vector2).LengthSquared() < MathF.Pow(Radius, 2);
        public bool Contains(Point point) => (new Vector2(X, Y) - point).LengthSquared() < MathF.Pow(Radius, 2);

        internal IEnumerable<Point> MakePolygon()
        {
            foreach (Vector2 point in GeometryServices.CreateCircle(Radius, 12))
            {
                yield return point.Point + new Point(X, Y);
            }
        }
    }
}
