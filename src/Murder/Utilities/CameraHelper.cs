using Microsoft.Xna.Framework;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Utilities
{
    public static class CameraHelper
    {
        public static (int minX, int maxX, int minY, int maxY) GetSafeGridBounds(this Camera2D camera, Map map) =>
            GetSafeGridBounds(camera, rect: new(x: 0, y: 0, width: map.Width, height: map.Height));

        public static (int minX, int maxX, int minY, int maxY) GetSafeGridBounds(this Camera2D camera, Rectangle rect)
        {
            int minX = Math.Max(rect.Left, Calculator.FloorToInt(camera.Bounds.Left / Grid.CellSize) - 2);
            int minY = Math.Max(rect.Top, Calculator.FloorToInt(camera.Bounds.Top / Grid.CellSize) - 2);

            int maxX = Math.Min(rect.Right, Calculator.CeilToInt(camera.Bounds.Right / Grid.CellSize) + 2);
            int maxY = Math.Min(rect.Bottom, Calculator.CeilToInt(camera.Bounds.Bottom / Grid.CellSize) + 2);

            return (minX, maxX, minY, maxY);
        }
    }
}