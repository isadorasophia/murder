using Bang.Entities;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Core
{
    public class TileGrid
    {
        [Bang.Serialize]
        [HideInEditor]
        private int[] _gridMap;

        [HideInEditor]
        private ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>> _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;

        [Bang.Serialize]
        [HideInEditor]
        private int _width = 1;

        [Bang.Serialize]
        [HideInEditor]
        private int _height = 1;

        [Bang.Serialize]
        [HideInEditor]
        private Point _origin = Point.Zero;

        private Point Size => new(_width, _height);
        public int Width => _width;
        public int Height => _height;
        public Point Origin => _origin;

        internal event Action? OnModified;

        [JsonConstructor]
        public TileGrid(Point origin, int width, int height)
        {
            (_width, _height) = (width, height);

            _origin = origin;
            _gridMap = new int[width * height];
        }

        public (int tile, int sortAdjust, bool occludeGround) GetTile(ImmutableArray<Entity> tileEntities, int index, int totalTilemaps, int x, int y)
        {
            if (x < 0 || y < 0 || x > Width || y > Height)
                return (-1, 0, false);

            CheckCacheAutoTile(tileEntities, totalTilemaps);

            return _tiles[index][y * (Width + 1) + x];
        }

        public void UpdateCache(ImmutableArray<Entity> tileEntities)
        {
            CacheAutoTile(tileEntities, _tiles.Length);
        }

        private void CheckCacheAutoTile(ImmutableArray<Entity> tileEntities, int totalTilemaps)
        {
            if (_tiles.Length == totalTilemaps)
            {
                // No need to cache
                return;
            }

            CacheAutoTile(tileEntities, totalTilemaps);

            OnModified?.Invoke();
        }

        private void CacheAutoTile(ImmutableArray<Entity> tileEntities, int totalTilemaps)
        {
            var builder = ImmutableArray.CreateBuilder<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>();
            
            for (int i = 0; i < totalTilemaps; i++)
            {
                int tileMask = i.ToMask();

                var layerBuilder = ImmutableArray.CreateBuilder<(int tile, int sortAdjust, bool occludeGround)>();
                for (int y = 0; y <= _height; y++)
                {
                    for (int x = 0; x <= _width; x++)
                    {
                        bool topLeft = TileServices.GetTileAt(tileEntities, this, x + Origin.X - 1, y + Origin.Y - 1, tileMask);
                        bool topRight = TileServices.GetTileAt(tileEntities, this, x + Origin.X, y + Origin.Y - 1, tileMask);
                        bool botLeft = TileServices.GetTileAt(tileEntities, this, x + Origin.X - 1, y + Origin.Y, tileMask);
                        bool botRight = TileServices.GetTileAt(tileEntities, this, x + Origin.X, y + Origin.Y, tileMask);

                        var tileCoordinate = TileServices.GetAutoTile(topLeft, topRight, botLeft, botRight);

                        bool occlude;

                        occlude =
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X, y + Origin.Y, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X + 1, y + Origin.Y, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X, y + Origin.Y + 1, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X + 1, y + Origin.Y + 1, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X - 1, y + Origin.Y - 1, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X - 1, y + Origin.Y, tileMask) &&
                            TileServices.GetTileAt(tileEntities, this, x + Origin.X, y + Origin.Y - 1, tileMask);

                        layerBuilder.Add((tileCoordinate.tile, tileCoordinate.sortAdjust, occlude));
                    }
                }

                builder.Add(layerBuilder.ToImmutable());
            }

            _tiles = builder.ToImmutable();
        }

        public int At(int x, int y)
        {
            if (x < 0 || y < 0) return 0;
            if (x >= Width || y >= Height) return 0;

            return _gridMap[(y * Width) + x];
        }

        public int At(Point p) => At(p.X, p.Y);

        public int AtGridPosition(Point p) => At(p - Origin);

        /// <summary>
        /// Checks whether is solid at a position <paramref name="x"/> and <paramref name="y"/>.
        /// This will take a position from the grid (world) back to the local grid, using <see cref="Origin"/>.
        /// </summary>
        public bool HasFlagAtGridPosition(int x, int y, int value) => HasFlagAt(x - Origin.X, y - Origin.Y, value);

        public virtual bool HasFlagAt(int x, int y, int value) => At(x, y).HasFlag(value);

        public void SetGridPosition(Point p, int value, bool overridePreviousValues = false) => Set(p - Origin, value, overridePreviousValues);

        public void UnsetGridPosition(Point p, int value) => Unset(p - Origin, value);

        public void Set(Point p, int value, bool overridePreviousValues = false) => Set(p.X, p.Y, value, overridePreviousValues);

        public void Unset(Point p, int value) => Unset(p.X, p.Y, value);

        public void SetGridPosition(IntRectangle rect, int value)
        {
            int miny = rect.Y - Origin.Y;
            int minx = rect.X - Origin.X;

            for (int cy = miny; cy < miny + rect.Height && cy < Height; cy++)
            {
                for (int cx = minx; cx < minx + rect.Width && cx < Width; cx++)
                {
                    Set(cx, cy, value);
                }
            }
        }

        public void UnsetGridPosition(IntRectangle rect, int value)
        {
            int miny = rect.Y - Origin.Y;
            int minx = rect.X - Origin.X;

            for (int cy = miny; cy < miny + rect.Height && cy < Height; cy++)
            {
                for (int cx = minx; cx < minx + rect.Width && cx < Width; cx++)
                {
                    Unset(cx, cy, value);
                }
            }
        }

        public void MoveFromTo(Point from, Point to, Point size)
        {
            int minyFrom = from.Y - Origin.Y;
            int minxFrom = from.X - Origin.X;

            int minyTo = to.Y - Origin.Y;
            int minxTo = to.X - Origin.X;

            for (int cy = 0; cy < size.Y /* height */ && cy + minyFrom < Height; cy++)
            {
                for (int cx = 0; cx < size.X /* width */ && cx + minxFrom < Width; cx++)
                {
                    int value = At(cx + minxFrom, cy + minyFrom);

                    Unset(cx + minxFrom, cy + minyFrom, value);
                    Set(cx + minxTo, cy + minyTo, value);
                }
            }
        }

        public void Set(int x, int y, int value, bool overridePreviousValues = false)
        {
            if (x < 0 || y < 0) return;

            if (overridePreviousValues)
            {
                _gridMap[(y * Width) + x] = value;
            }
            else
            {
                _gridMap[(y * Width) + x] |= value;
            }

            _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;

            OnModified?.Invoke();
        }

        public void Unset(int x, int y, int value)
        {
            _gridMap[(y * Width) + x] &= ~value;
            _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;

            OnModified?.Invoke();
        }

        /// <summary>
        /// Unset all the tiles according to the bitness of <paramref name="value"/>.
        /// </summary>
        public void UnsetAll(int value)
        {
            for (int cy = 0; cy < Height; cy++)
            {
                for (int cx = 0; cx < Width; cx++)
                {
                    _gridMap[cy * Width + cx] &= ~value;
                }
            }
            _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;
        }

        public void Resize(int width, int height, Point origin)
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

            int originDeltaY = Origin.Y - origin.Y;
            int originDeltaX = Origin.X - origin.X;

            // Convert (x, y) of the new copy to the current (x + deltaOrigin, y + deltaOrigin)
            (int x, int y) ConvertToCurrentPoint(int x, int y) => (x - originDeltaX, y - originDeltaY);

            for (int cy = 0; cy < height; cy++)
            {
                for (int cx = 0; cx < width; cx++)
                {
                    (int previousX, int previousY) = ConvertToCurrentPoint(cx, cy);

                    newArray[cy * width + cx] = At(previousX, previousY);
                }
            }

            _gridMap = newArray;
            _width = width;
            _height = height;

            _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;

            OnModified?.Invoke();
        }

        /// <summary>
        /// This supports resize the grid up to:
        ///   _____      ______
        ///  |     | -> |      |
        ///  |_____x    |      |
        ///             |______x
        /// or
        ///   _____      _____
        ///  |  x  | -> |  x  |
        ///  |_____|    |_____|
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

            if (rectangle.Size != Size)
            {
                Resize(rectangle.Width, rectangle.Height, origin: rectangle.TopLeft);
            }

            if (rectangle.TopLeft != Origin)
            {
                _origin = rectangle.TopLeft;
            }

            _tiles = ImmutableArray<ImmutableArray<(int tile, int sortAdjust, bool occludeGround)>>.Empty;

            OnModified?.Invoke();
        }
    }
}