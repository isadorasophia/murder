using Microsoft.Xna.Framework;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Utilities
{
    public static class CameraHelper
    {
        public static (int minX, int maxX, int minY, int maxY) GetSafeGridBounds(this Camera2D camera, Map map) =>
            GetSafeGridBounds(camera, rect: new(x: 0, y: 0, width: map.Width, height: map.Height));

        public static (int minX, int maxX, int minY, int maxY) GetSafeGridBounds(this Camera2D camera, Rectangle rect, int extraPadding = 2)
        {
            int minX = Math.Max(rect.Left, Calculator.FloorToInt(camera.Bounds.Left / Grid.CellSize) - extraPadding);
            int minY = Math.Max(rect.Top, Calculator.FloorToInt(camera.Bounds.Top / Grid.CellSize) - extraPadding);

            int maxX = Math.Min(rect.Right, Calculator.CeilToInt(camera.Bounds.Right / Grid.CellSize) + extraPadding);
            int maxY = Math.Min(rect.Bottom, Calculator.CeilToInt(camera.Bounds.Bottom / Grid.CellSize) + extraPadding);

            return (minX, maxX, minY, maxY);
        }
    }
}