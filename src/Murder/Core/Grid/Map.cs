using Murder.Core.Geometry;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Core
{
    public class Map
    {
        private readonly object _lock = new();

        [JsonProperty]
        private readonly MapTile[] _gridMap;

        public MapTile GetGridMap(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                MapTile outOfBoundsTile = new();
                outOfBoundsTile.CollisionType = GridCollisionType.Static | GridCollisionType.BlockVision;

                return outOfBoundsTile;
            }

            return _gridMap[(y * Width) + x];
        }

        private ref MapTile GetGridMapRef(int x, int y) => ref _gridMap[(y * Width) + x];

        public readonly int Width;
        public readonly int Height;

        [JsonConstructor]
        public Map(int width, int height)
        {
            (Width, Height) = (width, height);

            _gridMap = new MapTile[width * height];

            Array.Fill(_gridMap, new());
        }

        public void Explore(int x, int y)
        {
            if (IsInsideGrid(x, y))
            {
                if (GetGridMapRef(x, y).VisionStatus == GridVisionStatus.None)
                {
                    GetGridMapRef(x, y).VisionStatus = GridVisionStatus.Explored;
                    GetGridMapRef(x, y).ExploredAt = Time.Elapsed;
                }
            }
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
        public bool HasLineOfSight(Point start, Point end, bool excludeEdges, GridCollisionType blocking = GridCollisionType.BlockVision)
        {
            return HasLineOfSight(start, end, excludeEdges, (x, y) => GetCollision(x, y).HasFlag(blocking));
        }

        /// <summary>
        /// A fast Line of Sight check
        /// It is not exact by any means, just tries to draw A line of tiles between start and end.
        /// </summary>
        /// <param name="start">Starting tile</param>
        /// <param name="end">End tile</param>
        /// <param name="excludeEdges">Exclude starting and end tiles</param>
        /// <param name="filter">The method to check for obsctacle. True means an obstacle was reached</param>
        /// <returns></returns>
        private bool HasLineOfSight(Point start, Point end, bool excludeEdges, Func<int,int,bool> filter)
        {
            var line = GridHelper.Line(start, end.Clamp(0,0,Width,Height)).ToImmutableArray(); // Eeeehh I don't like this
            bool isWall = GetCollision(end.X, end.Y).HasFlag(GridCollisionType.Static);

            for (int i = 0; i < line.Length; i++)
            {
                var grid = line[i];

                if (excludeEdges && (grid == end || grid == start))
                    continue;
                if (filter(grid.X, grid.Y))
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasAnyCollision(int x, int y)
            => HasCollision(x, y, GridCollisionType.Static | GridCollisionType.Carve);

        public bool HasStaticCollision(int x, int y)
            => HasCollision(x, y, GridCollisionType.Static);

        public bool HasCarveCollision(int x, int y)
            => HasCollision(x, y, GridCollisionType.Carve);

        public bool HasCollision(int x, int y, GridCollisionType type)
        {
            return (GetCollision(x, y) & type) != GridCollisionType.None;
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

        public GridCollisionType GetCollision(int x, int y)
        {
            if (!IsInsideGrid(x, y))
                return GridCollisionType.Static | GridCollisionType.BlockVision;

            return At(x, y);
        }

        public bool HasCollision(int x, int y, int width, int height, GridCollisionType type)
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
                    if (At(cx, cy).HasFlag(type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Point? HasCollisionAt(IntRectangle rect, GridCollisionType type) =>
            HasCollisionAt(rect.X, rect.Y, rect.Width, rect.Height, type);

        /// <summary>
        /// Check for collision using tiles coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Point? HasStaticCollisionAt(int x, int y, int width, int height) =>
            HasCollisionAt(x, y, width, height, GridCollisionType.Static);

        /// <summary>
        /// Check for collision using tiles coordinates.
        /// </summary>
        public Point? HasCollisionAt(int x, int y, int width, int height, GridCollisionType type)
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

                    if (At(cx, cy).HasFlag(type))
                    {
                        return new(cx, cy);
                    }
                }
            }

            return null;
        }

        public void SetOccupiedAsStatic(int x, int y)
        {
            _gridMap[(y * Width) + x].CollisionType |= GridCollisionType.Static | GridCollisionType.BlockVision | GridCollisionType.IsObstacle;
        }

        public void SetOccupiedAsCarve(IntRectangle rect, bool blockVision, bool isObstacle, int weight)
        {
            GridCollisionType collisionMask = GridCollisionType.Carve;
            if (blockVision)
            {
                collisionMask |= GridCollisionType.BlockVision;
            }

            if (isObstacle)
            {
                collisionMask |= GridCollisionType.IsObstacle;
            }

            SetGridCollision(rect.X, rect.Y, rect.Width, rect.Height, collisionMask, weight);
        }

        public void SetUnoccupiedCarve(IntRectangle  rect, bool blockVision, bool isObstacle, int weight)
        {
            GridCollisionType collisionMask = GridCollisionType.Carve;
            if (blockVision)
            {
                collisionMask |= GridCollisionType.BlockVision;
            }

            if (isObstacle)
            {
                collisionMask |= GridCollisionType.IsObstacle;
            }

            UnsetGridCollision(rect.X, rect.Y, rect.Width, rect.Height, collisionMask, weight);
        }

        private void UnsetGridCollision(int x, int y, int width, int height, GridCollisionType value, int weight)
        {
            for (int cy = y; cy < y + height; cy++)
            {
                for (int cx = x; cx < x + width; cx++)
                {
                    int position = (cy * Width) + cx;
                    _gridMap[position].CollisionType &= ~value;

                    if (!value.HasFlag(GridCollisionType.IsObstacle))
                    {
                        Debug.Assert(weight != -1);

                        // Clamp weight at 1.
                        _gridMap[position].Weight = Math.Max(1, _gridMap[position].Weight - weight);
                    }
                }
            }
        }

        private void SetGridCollision(int x, int y, int width, int height, GridCollisionType value, int weight = -1)
        {
            for (int cy = y; cy < y + height && cy < Height; cy++)
            {
                for (int cx = x; cx < x + width && cx < Width; cx++)
                {
                    int position = (cy * Width) + cx;
                    if (position>=0 && position<_gridMap.Length)
                        _gridMap[position].CollisionType |= value;

                    if (!value.HasFlag(GridCollisionType.IsObstacle))
                    {
                        Debug.Assert(weight != -1);
                        _gridMap[position].Weight += weight;
                    }
                }
            }
        }

        private GridCollisionType At(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                return GridCollisionType.Static | GridCollisionType.BlockVision;
            }
            
            lock (_lock)
            {
                return _gridMap[(y * Width) + x].CollisionType;
            }
        }

        public IEnumerable<Point> GetStaticCollisions(IntRectangle rect) => GetCollisionsWith(rect.X, rect.Y, rect.Width, rect.Height, GridCollisionType.Static);
        
        public IEnumerable<Point> GetVisionCollisions(IntRectangle rect) => GetCollisionsWith(rect.X, rect.Y, rect.Width, rect.Height, GridCollisionType.BlockVision);

        public bool IsObstacleOrBlockVision(Point p)
        {
            GridCollisionType target = At(p.X, p.Y);
            return target.HasFlag(GridCollisionType.IsObstacle) || target.HasFlag(GridCollisionType.BlockVision);
        }

        public bool IsObstacle(Point p) => At(p.X, p.Y).HasFlag(GridCollisionType.IsObstacle);
        
        public int WeightAt(int x, int y)
        {
            if (!IsInsideGrid(x, y))
            {
                return -1;
            }

            lock (_lock)
            {
                return _gridMap[(y * Width) + x].Weight;
            }
        }

        internal IEnumerable<Point> GetCollisionsWith(int x, int y, int width, int height, GridCollisionType gridCollisionType)
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
                    else if (At(cx, cy).HasFlag(gridCollisionType))
                    {
                        yield return new(cx, cy);
                    }
                }
            }

        }
    }
}
