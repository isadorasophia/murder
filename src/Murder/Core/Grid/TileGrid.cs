using Newtonsoft.Json;

namespace Murder.Core
{
    public struct TileGrid
    {
        [JsonProperty]
        private readonly int[] _gridMap;

        public readonly int Width;
        public readonly int Height;

        [JsonConstructor]
        public TileGrid(int width, int height)
        {
            (Width, Height) = (width, height);

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
    }
}
