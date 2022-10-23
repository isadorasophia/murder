using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Core
{
    public static class Grid
    {
        public const int CellSize = 24;
        public const int HalfCell = 12;

        public const int Width = 256;
        public const int Height = 256;

        public static readonly Point Dimensions = new(Width, Height);
        public static readonly Point CellDimensions = new(CellSize, CellSize);

        public static readonly Point HalfCellDimensions = 
            new(Calculator.RoundToInt(CellSize / 2), Calculator.RoundToInt(CellSize / 2));

        public static int FloorToGrid(float value) => Calculator.FloorToInt(value / CellSize);
        public static int RoundToGrid(float value) => Calculator.RoundToInt(value / CellSize);
        public static int CeilToGrid(float value) => Calculator.CeilToInt(value / CellSize);
    }
}
