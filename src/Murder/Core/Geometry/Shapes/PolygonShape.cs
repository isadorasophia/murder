namespace Murder.Core.Geometry
{
    public struct PolygonShape : IShape
    {
        public readonly Polygon Polygon = new Polygon();

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

                int left = Polygon.Vertices[_leftIndex].X;
                int right = Polygon.Vertices[_rightIndex].X;
                int top = Polygon.Vertices[_topIndex].Y;
                int bottom = Polygon.Vertices[_bottomIndex].Y;
                return new Rectangle(
                    left, top, right - left, bottom - top
                );
            }
        }

        public PolygonShape() { }
        public PolygonShape(Polygon polygon) {
            Polygon = polygon;
        }

        public void Cache()
        {
            var leftMost = int.MaxValue;
            var rightMost = int.MinValue;
            var topMost = int.MaxValue;
            var bottomMost = int.MinValue;

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
    }
}
