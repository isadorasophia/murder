using Assimp;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Newtonsoft.Json;

namespace Murder.Core
{
    public struct TileGrid
    {
        [JsonProperty]
        private int[] _gridMap;

        [JsonProperty]
        private int _width = 1;

        [JsonProperty]
        private int _height = 1;

        [JsonProperty]
        private Point _origin = Point.Zero;

        private Point Size => new(_width, _height);

        public int Width => _width;
        public int Height => _height;
        public Point Origin => _origin;

        [JsonConstructor]
        public TileGrid(int width, int height)
        {
            (_width, _height) = (width, height);

            _gridMap = new int[width * height];
        }

        public int At(int x, int y)
        {
            return _gridMap[(y * Width) + x];
        }

        public void Set(int x, int y, int value)
        {
            _gridMap[(y * Width) + x] = value;
        }

        public void Resize(int width, int height) 
        {
            if (width < 0 || height < 0)
            {
                GameLogger.Error($"Invalid size when resizing grid: {width}, {height}.");
                return;
            }

            if (width == Width && height == Height)
            {
                return;
            }

            int[] newArray = new int[width * height];
            Array.Copy(_gridMap, newArray, Math.Min(_gridMap.Length, newArray.Length));

            _gridMap = newArray;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// This supports resize the grid up to:
        ///   _____      ______
        ///  |     | -> |      |
        ///  |_____x    |      |
        ///             |______x
        /// or
        ///   _____         _____
        ///  |  x  | ->    |  x  |
        ///  |_____|       |_____|
        /// 
        /// Where x is the bullet point.
        /// </summary>
        /// <param name="rectangle"></param>
        public void Resize(IntRectangle rectangle)
        {
            if (rectangle.TopLeft == Origin && rectangle.Size == Size)
            {
                return;
            }

            if (rectangle.TopLeft != Origin)
            {
                _origin = rectangle.TopLeft;
            }

            if (rectangle.Size != Size)
            {
                Resize(rectangle.Width, rectangle.Height);
            }
        }
    }
}
