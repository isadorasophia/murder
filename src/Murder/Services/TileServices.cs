using Bang.Entities;
using Murder.Core;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public static Point GetAutoTile(bool topLeft, bool topRight, bool botLeft, bool botRight)
        {
            // Top Left 
            if (!topLeft && !topRight && !botLeft && botRight)
                return new(0, 0);

            // Top
            if (!topLeft && !topRight && botLeft && botRight)
                return new(1, 0);

            // Top Right
            if (!topLeft && !topRight && botLeft && !botRight)
                return new(2, 0);

            // Left 
            if (!topLeft && topRight && !botLeft && botRight)
                return new(0, 1);

            // Full tile
            if (topLeft && topRight && botLeft && botRight)
                return new(1, 1);

            // Right
            if (topLeft && !topRight && botLeft && !botRight)
                return new(2, 1);

            // Bottom Left 
            if (!topLeft && topRight && !botLeft && !botRight)
                return new(0, 2);

            // Bottom
            if (topLeft && topRight && !botLeft && !botRight)
                return new(1, 2);

            // Bottom Right
            if (topLeft && !topRight && !botLeft && !botRight)
                return new(2, 2);

            // Top Left Inside Corner
            if (topLeft && topRight && botLeft && !botRight)
                return new(1, 3);

            // Top Right Inside Corner
            if (topLeft && topRight && !botLeft && botRight)
                return new(2, 3);

            // Top Left Inside Corner
            if (topLeft && !topRight && botLeft && botRight)
                return new(1, 4);

            // Top Right Inside Corner
            if (!topLeft && topRight && botLeft && botRight)
                return new(2, 4);

            // Diagonal Down Up
            if (topLeft && !topRight && !botLeft && botRight)
                return new(0, 3);

            // Diagonal Up Down
            if (!topLeft && topRight && botLeft && !botRight)
                return new(0, 4);

            return new(-1, -1);
        }
    }
}
