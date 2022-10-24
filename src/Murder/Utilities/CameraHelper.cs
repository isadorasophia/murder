using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Utilities
{
    internal static class CameraHelper
    {
        public static (int minX, int maxX, int minY, int maxY) GetSafeGridBounds(this Camera2D camera, Map map)
        {
            int minX = Math.Max(0, Calculator.FloorToInt(camera.Bounds.Left / Grid.CellSize) - 2);
            int maxX = Math.Min(map.Width + 1, Calculator.CeilToInt(camera.Bounds.Right / Grid.CellSize) + 2);

            int minY = Math.Max(0, Calculator.FloorToInt(camera.Bounds.Top / Grid.CellSize) - 2);
            int maxY = Math.Min(map.Height + 1, Calculator.CeilToInt(camera.Bounds.Bottom / Grid.CellSize) + 2);

            return (minX, maxX, minY, maxY);
        }
    }
}