using Murder.Utilities;

namespace Murder.Core.Geometry
{
    public record struct PolygonShape : IShape
    {
        public readonly Polygon Polygon = Polygon.DIAMOND;

        private int _leftIndex = -1;
        private int _topIndex = -1;
        private int _bottomIndex = -1;
        private int _rightIndex = -1;

        public Rectangle Rect
        {
            get
            {
                if (_leftIndex == -1)
                    Cache();
                if (_leftIndex == -1)
                    return Rectangle.Empty;

                float left = Polygon.Vertices[_leftIndex].X;
                float right = Polygon.Vertices[_rightIndex].X;
                float top = Polygon.Vertices[_topIndex].Y;
                float bottom = Polygon.Vertices[_bottomIndex].Y;
                return new Rectangle(
                    left, top, right - left, bottom - top
                );
            }
        }

        public PolygonShape() { }
        public PolygonShape(Polygon polygon)
        {
            Polygon = polygon;
        }

        public void Cache()
        {
            float leftMost = int.MaxValue;
            float rightMost = int.MinValue;
            float topMost = int.MaxValue;
            float bottomMost = int.MinValue;

            for (int i = 0; i < Polygon.Vertices.Length; i++)
            {
                var point = Polygon.Vertices[i];

                if (point.X < leftMost)
                {
                    leftMost = point.X;
                    _leftIndex = i;
                }

                if (point.X > rightMost)
                {
                    rightMost = point.X;
                    _rightIndex = i;
                }

                if (point.Y < topMost)
                {
                    topMost = point.Y;
                    _topIndex = i;
                }

                if (point.Y > bottomMost)
                {
                    bottomMost = point.Y;
                    _bottomIndex = i;
                }
            }
        }
        public Rectangle GetBoundingBox() => Rect;

        public Point GetCenter() => Rect.CenterPoint;

        public PolygonShape GetPolygon()
        {
            return this;
        }
    }
}