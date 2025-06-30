using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Murder.Core
{
    public class Map
    {
        public record TileCollisionInfo(IntRectangle Rectangle, int Layer);

        private readonly object _lock = new();

        [Bang.Serialize]
        private readonly MapTile[] _gridMap;

        /// <summary>
        /// Map all the properties of the floor based on an arbitrary enum (defined by a game implementation).
        /// </summary>
        [Bang.Serialize]
        private readonly int[] _floorMap;

        public MapTile GetGridMap(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                MapTile outOfBoundsTile = new();
                outOfBoundsTile.CollisionType = CollisionLayersBase.SOLID;

                return outOfBoundsTile;
            }

            return _gridMap[(y * Width) + x];
        }

        private ref MapTile GetGridMapRef(int x, int y) => ref _gridMap[(y * Width) + x];

        /// <summary>
        /// This should not be added to <see cref="Width"/> and <see cref="Height"/>.
        /// This only tracks the origin bounds of the map for purposes of visualization.
        /// </summary>
        public readonly Point Origin;

        public readonly int Width;
        public readonly int Height;

        [JsonConstructor]
        public Map(Point origin, int width, int height)
        {
            (Width, Height) = (width, height);
            Origin = origin;

            _gridMap = new MapTile[width * height];
            _floorMap = new int[width * height];

            Array.Fill(_gridMap, new());
        }

        public void ZeroAll()
        {
            Array.Fill(_gridMap, new(weight: 0));
        }

        /// <summary>
        /// A fast Line of Sight check
        /// It is not exact by any means, just tries to draw A line of tiles between start and end.
        /// </summary>
        /// <param name="start">Starting tile</param>
        /// <param name="end">End tile</param>
        /// <param name="excludeEdges">Exclude starting and end tiles</param>
        /// <param name="blocking">Blocking tiles</param>
        /// <returns></returns>
        public bool HasLineOfSight(Point start, Point end, bool excludeEdges, int blocking)
        {
            // Directly iterate over the IEnumerable<Point> without converting to an ImmutableArray
            foreach (var grid in GridHelper.Line(start, end.Clamp(0, 0, Width, Height)))
            {
                if (excludeEdges && (grid == end || grid == start))
                    continue;
                if (At(grid.X, grid.Y).HasFlag(blocking))
                {
                    return false;
                }
            }

            return true;
        }


        public bool HasCollision(int x, int y, int layer)
        {
            return (At(x, y) & layer) != CollisionLayersBase.NONE;
        }

        public bool IsInsideGrid(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }

            if (x >= Width || y >= Height)
            {
                return false;
            }
            return true;
        }

        public bool HasCollision(int x, int y, int width, int height, int mask)
        {
            if (x < 0 || y < 0 || width < 0 || height < 0)
            {
                return true;
            }

            if (x > Width || y > Height)
            {
                return true;
            }

            for (int cy = y; cy < y + height; cy++)
            {
                for (int cx = x; cx < x + width; cx++)
                {
                    if (At(cx, cy).HasFlag(mask))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Point? HasCollisionAt(IntRectangle rect, int mask) =>
            HasCollisionAt(rect.X, rect.Y, rect.Width, rect.Height, mask);

        /// <summary>
        /// Check for collision using tiles coordinates.
        /// </summary>
        public Point? HasCollisionAt(int x, int y, int width, int height, int mask)
        {
            if (width < 0 || height < 0)
            {
                return null;
            }

            for (int cy = y; cy < y + height && cy < Height; cy++)
            {
                for (int cx = x; cx < x + width && cx < Width; cx++)
                {
                    if (cx < 0)
                    {
                        if (cy < 0)
                        {
                            return new(-1, -1);
                        }
                        else
                        {
                            return new(-1, cy);
                        }
                    }

                    if (cy < 0)
                    {
                        return new(cx, -1);
                    }

                    if (cx > Width || cy > Height)
                    {
                        return new(cx, cy);
                    }

                    if (At(cx, cy).HasFlag(mask))
                    {
                        return new(cx, cy);
                    }
                }
            }

            return null;
        }

        public void SetOccupiedAsStatic(int x, int y, int layer, bool @override = false)
        {
            if (@override)
            {
                _gridMap[(y * Width) + x].CollisionType &= layer;
            }
            else
            {
                _gridMap[(y * Width) + x].CollisionType |= layer;
            }
        }

        public void SetOccupied(Point p, int collisionMask, int weight) =>
            SetGridCollision(p.X, p.Y, 1, 1, collisionMask, @override: false, weight);

        public void SetUnoccupied(Point p, int collisionMask, int weight) =>
            UnsetGridCollision(p.X, p.Y, 1, 1, collisionMask, weight);

        public void SetOccupiedAsCarve(IntRectangle rect, bool blockVision, bool isObstacle, bool isClearPath, int weight)
        {
            int collisionMask = CollisionLayersBase.CARVE;

            if (isClearPath)
            {
                collisionMask = CollisionLayersBase.NONE;
            }

            if (blockVision)
            {
                collisionMask |= CollisionLayersBase.BLOCK_VISION;
            }

            if (isObstacle)
            {
                collisionMask |= CollisionLayersBase.SOLID;
            }

            SetGridCollision(rect.X, rect.Y, rect.Width, rect.Height, collisionMask, @override: isClearPath, weight);
        }

        public void SetUnoccupiedCarve(IntRectangle rect, bool blockVision, bool isObstacle, int weight)
        {
            int collisionMask = CollisionLayersBase.CARVE;
            if (blockVision)
            {
                collisionMask |= CollisionLayersBase.BLOCK_VISION;
            }

            if (isObstacle)
            {
                collisionMask |= CollisionLayersBase.SOLID;
            }

            UnsetGridCollision(rect.X, rect.Y, rect.Width, rect.Height, collisionMask, weight);
        }

        private void UnsetGridCollision(int x, int y, int width, int height, int value, int weight)
        {
            if (x < 0 || y < 0 || x > Width || y > Height)
            {
                return;
            }

            for (int cy = y; cy < y + height && cy < Height; cy++)
            {
                for (int cx = x; cx < x + width && cx < Width; cx++)
                {
                    int position = (cy * Width) + cx;
                    _gridMap[position].CollisionType &= ~value;

                    if (!value.HasFlag(CollisionLayersBase.SOLID))
                    {
                        Debug.Assert(weight != -1);

                        // Clamp weight at 1.
                        _gridMap[position].Weight = Math.Max(1, _gridMap[position].Weight - weight);
                    }
                }
            }
        }

        private void SetGridCollision(int x, int y, int width, int height, int layer, bool @override, int weight = -1)
        {
            if (x < 0 || y < 0 || x > Width || y > Height)
            {
                return;
            }

            for (int cy = y; cy < y + height && cy < Height; cy++)
            {
                for (int cx = x; cx < x + width && cx < Width; cx++)
                {
                    int position = Math.Clamp((cy * Width) + cx, 0, _gridMap.Length);
                    if (position >= 0 && position < _gridMap.Length)
                    {
                        if (@override)
                        {
                            _gridMap[position].CollisionType = layer;
                        }
                        else
                        {
                            _gridMap[position].CollisionType |= layer;
                        }
                    }

                    if (!layer.HasFlag(CollisionLayersBase.SOLID))
                    {
                        Debug.Assert(weight != -1);
                        _gridMap[position].Weight += weight;
                    }
                }
            }
        }

        public int At(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                return CollisionLayersBase.SOLID | CollisionLayersBase.BLOCK_VISION;
            }

            lock (_lock)
            {
                return _gridMap[(y * Width) + x].CollisionType;
            }
        }

        public IEnumerable<Point> GetStaticCollisions(IntRectangle rect) => GetCollisionsWith(rect.X, rect.Y, rect.Width, rect.Height, CollisionLayersBase.SOLID);

        public bool IsObstacleOrBlockVision(Point p)
        {
            int target = At(p.X, p.Y);
            return target.HasFlag(CollisionLayersBase.SOLID | CollisionLayersBase.HOLE | CollisionLayersBase.BLOCK_VISION);
        }

        public bool IsObstacle(Point p) => At(p.X, p.Y).HasFlag(CollisionLayersBase.SOLID | CollisionLayersBase.HOLE);

        public int WeightAt(Point p) => WeightAt(p.X, p.Y);

        public int WeightAt(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                return 100;
            }

            lock (_lock)
            {
                return _gridMap[(y * Width) + x].Weight;
            }
        }

        public void OverrideValueAt(Point p, int collisionMask, int weight)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height)
            {
                return;
            }

            _gridMap[(p.Y * Width) + p.X].CollisionType = collisionMask;
            _gridMap[(p.Y * Width) + p.X].Weight = weight;
        }

        public void SetFloorAt(int x, int y, int type)
        {
            _floorMap[(y * Width) + x] = type;
        }

        public void SetFloorAt(IntRectangle rect, int type)
        {
            (int x, int y, int width, int height) = (rect.X, rect.Y, rect.Width, rect.Height);

            for (int cy = y; cy <= y + height && cy < Height; cy++)
            {
                for (int cx = x; cx <= x + width && cx < Width; cx++)
                {
                    int position = (cy * Width) + cx;
                    _floorMap[position] = type;
                }
            }
        }

        public int FloorAt(Point p)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height)
            {
                return -1;
            }
            return _floorMap[(p.Y * Width) + p.X];
        }
        internal IEnumerable<TileCollisionInfo> GetCollisionInfosWith(int x, int y, int width, int height, int mask)
        {
            for (int cy = Math.Max(-1, y); cy < y + height; cy++)
            {
                for (int cx = Math.Max(-1, x); cx < x + width; cx++)
                {
                    if (cx < 0)
                    {
                        if (cy < 0)
                        {
                            yield return new(new(-1, -1, 1, 1), CollisionLayersBase.NONE);
                        }
                        else
                        {
                            yield return new(new(-1, cy, 1, 1), CollisionLayersBase.NONE);
                        }
                    }
                    else if (cy < 0)
                    {
                        yield return new(new(cx, -1, 1, 1), CollisionLayersBase.NONE);
                    }
                    else if (cx >= Width)
                    {
                        if (cy >= Height)
                        {
                            yield return new(new(Width, Height, 1, 1), CollisionLayersBase.NONE);
                        }
                        else
                        {
                            yield return new(new(Width, cy, 1, 1), CollisionLayersBase.NONE);
                        }
                    }
                    else if (cy >= Height)
                    {
                        yield return new(new(cx, Height, 1, 1), CollisionLayersBase.NONE);
                    }
                    else
                    {
                        var at = At(cx, cy);
                        if (at.HasFlag(mask))
                        {
                            yield return new(new(cx, cy, 1, 1), at);
                        }
                    }
                }
            }

        }
        internal IEnumerable<Point> GetCollisionsWith(int x, int y, int width, int height, int mask)
        {
            for (int cy = Math.Max(-1, y); cy < y + height; cy++)
            {
                for (int cx = Math.Max(-1, x); cx < x + width; cx++)
                {
                    if (cx < 0)
                    {
                        if (cy < 0)
                        {
                            yield return new(-1, -1);
                        }
                        else
                        {
                            yield return new(-1, cy);
                        }
                    }
                    else if (cy < 0)
                    {
                        yield return new(cx, -1);
                    }
                    else if (cx >= Width)
                    {
                        if (cy >= Height)
                        {
                            yield return new(Width, Height);
                        }
                        else
                        {
                            yield return new(Width, cy);
                        }
                    }
                    else if (cy >= Height)
                    {
                        yield return new(cx, Height);
                    }
                    else if (At(cx, cy).HasFlag(mask))
                    {
                        yield return new(cx, cy);
                    }
                }
            }

        }
    }
}