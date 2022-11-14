using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Core
{
    public static class Grid
    {
        public const int CellSize = 24;
        public const int HalfCell = 12;

        public static readonly Point CellDimensions = new(CellSize, CellSize);

        public static readonly Point HalfCellDimensions = 
            new(Calculator.RoundToInt(CellSize / 2), Calculator.RoundToInt(CellSize / 2));

        public static int FloorToGrid(float value) => Calculator.FloorToInt(value / CellSize);
        public static int RoundToGrid(float value) => Calculator.RoundToInt(value / CellSize);
        public static int CeilToGrid(float value) => Calculator.CeilToInt(value / CellSize);

        public static int ToMask(this int value) => 1 << value;

        public static bool HasFlag(this int value, int mask) => (value & mask) != 0;
    }
}
