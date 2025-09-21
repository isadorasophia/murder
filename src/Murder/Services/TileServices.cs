using Bang.Entities;
using Murder.Core;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Services
{
    internal class TileServices
    {
        public static bool GetTileAt(ImmutableArray<Entity> tilemaps, TileGrid grid, int x, int y, int tileMask)
        {
            bool isOffLimits = x - grid.Origin.X < 0 || x - grid.Origin.X >= grid.Width || y - grid.Origin.Y < 0 || y - grid.Origin.Y >= grid.Height;

            if (!isOffLimits)
            {
                // Easy scenario, just look for current grid.
                return grid.HasFlagAtGridPosition(x, y, tileMask);
            }

            // Since this is not in the current room, we should look if it's in the other tilemap rooms.
            for (int i = 0; i < tilemaps.Length; i++)
            {
                if (tilemaps[i].GetTileGrid().Grid.HasFlagAtGridPosition(x, y, tileMask))
                {
                    return true;
                }
            }

            return false;
        }

        public static (int tile, int sortAdjust) GetAutoTile(bool topLeft, bool topRight, bool botLeft, bool botRight)
        {
            // Top Left 
            if (!topLeft && !topRight && !botLeft && botRight)
                return (Calculator.OneD(0, 0, 3), 1);

            // Top
            if (!topLeft && !topRight && botLeft && botRight)
                return (Calculator.OneD(1, 0, 3), 1);

            // Top Right
            if (!topLeft && !topRight && botLeft && !botRight)
                return (Calculator.OneD(2, 0, 3), 1);

            // Left 
            if (!topLeft && topRight && !botLeft && botRight)
                return (Calculator.OneD(0, 1, 3), 0);

            // Full tile
            if (topLeft && topRight && botLeft && botRight)
                return (Calculator.OneD(1, 1, 3), 0);

            // Right
            if (topLeft && !topRight && botLeft && !botRight)
                return (Calculator.OneD(2, 1, 3), 0);

            // Bottom Left 
            if (!topLeft && topRight && !botLeft && !botRight)
                return (Calculator.OneD(0, 2, 3), 0);

            // Bottom
            if (topLeft && topRight && !botLeft && !botRight)
                return (Calculator.OneD(1, 2, 3), 0);

            // Bottom Right
            if (topLeft && !topRight && !botLeft && !botRight)
                return (Calculator.OneD(2, 2, 3), 0);

            // Top Left Inside Corner
            if (topLeft && topRight && botLeft && !botRight)
                return (Calculator.OneD(1, 3, 3), 0);

            // Top Right Inside Corner
            if (topLeft && topRight && !botLeft && botRight)
                return (Calculator.OneD(2, 3, 3), 0);

            // Bottom Left Inside Corner
            if (topLeft && !topRight && botLeft && botRight)
                return (Calculator.OneD(1, 4, 3), 1);

            // Bottom Right Inside Corner
            if (!topLeft && topRight && botLeft && botRight)
                return (Calculator.OneD(2, 4, 3), 1);

            // Diagonal Down Up
            if (topLeft && !topRight && !botLeft && botRight)
                return (Calculator.OneD(0, 3, 3), 0);

            // Diagonal Up Down
            if (!topLeft && topRight && botLeft && !botRight)
                return (Calculator.OneD(0, 4, 3), 0);

            return new(-1, -1);
        }
    }
}