using Murder.Core.Geometry;
using Murder.Utilities;

namespace Murder.Core
{
    /// <summary>
    /// Helper static class that forwards the values of the default <see cref="Game.Grid"/> for easier access.
    /// </summary>
    public static class Grid
    {
        public static int CellSize => Game.Grid.CellSize;
        public static int HalfCellSize => Game.Grid.HalfCellSize;
        public static Point CellDimensions => Game.Grid.CellDimensions;
        public static Point HalfCellDimensions => Game.Grid.HalfCellDimensions;

        public static int FloorToGrid(float value) => Game.Grid.FloorToGrid(value);
        public static int RoundToGrid(float value) => Game.Grid.RoundToGrid(value);
        public static int CeilToGrid(float value) => Game.Grid.CeilToGrid(value);
    }

    public readonly struct GridConfiguration
    {
        /// <summary>
        /// Size of this grid's individual cell.
        /// </summary>
        public readonly int CellSize;

        /// <summary>
        /// <see cref="CellSize"/> divided by two.
        /// </summary>
        public readonly int HalfCellSize;

        /// <summary>
        /// A point that is <see cref="CellSize"/> by <see cref="CellSize"/>.
        /// </summary>
        public readonly Point CellDimensions;

        /// <summary>
        /// A point that is <see cref="HalfCellSize"/> by <see cref="HalfCellSize"/>.
        /// </summary>
        public readonly Point HalfCellDimensions;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="cellSize">Basic value used for calculating all useful cell size values.</param>
        public GridConfiguration(int cellSize)
        {
            CellSize = cellSize;
            HalfCellSize = Calculator.RoundToInt(CellSize / 2.0f);
            CellDimensions = new(CellSize, CellSize);
            HalfCellDimensions = new(HalfCellSize, HalfCellSize);
        }

        /// <summary>
        /// Find in which cell of the grid a value would land, rounding down.
        /// </summary>
        /// <param name="value">The point in the grid</param>
        /// <returns>The cell this would land at.</returns>
        public int FloorToGrid(float value) => value % CellSize == 0 ? Calculator.RoundToInt(value / CellSize) : Calculator.FloorToInt(value / CellSize);

        /// <summary>
        /// Find in which cell of the grid a value would land, with default <see cref="Calculator.RoundToInt(float)"/> behavior.
        /// </summary>
        /// <param name="value">The point in the grid</param>
        /// <returns>The cell this would land at.</returns>
        public int RoundToGrid(float value) => Calculator.RoundToInt(value / CellSize);

        /// <summary>
        /// Find in which cell of the grid a value would land, rounding up.
        /// </summary>
        /// <param name="value">The point in the grid</param>
        /// <returns>The cell this would land at.</returns>
        public int CeilToGrid(float value) => value % CellSize == 0 ? Calculator.RoundToInt(value / CellSize) : Calculator.CeilToInt(value / CellSize);
    }

    /// <summary>
    /// Numeric extensions that are grid related.
    /// </summary>
    public static class GridNumberExtensions
    {
        public static int ToMask(this int value) => 1 << value;

        public static bool HasFlag(this int value, int mask) => (value & mask) != 0;
    }
}