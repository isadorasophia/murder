using Murder.Utilities;
using Murder.Attributes;

namespace Murder.Core.Geometry
{
    public readonly struct CircleShape : IShape
    {
        public readonly float Radius;

        [Slider]
        public readonly Point Offset;

        public Circle Circle => new(Offset.X, Offset.Y, Radius);

        public CircleShape(float radius, Point offset) => (Radius, Offset) = (radius, offset);

        public Rectangle GetBoundingBox()
        {
            int radius = Calculator.RoundToInt(Radius);
            int diameter= Calculator.RoundToInt(Radius * 2);
            return new Rectangle(Offset.X - radius, Offset.Y - radius, diameter, diameter);
        }
    }
}
