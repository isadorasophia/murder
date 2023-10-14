﻿using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Murder.Core
{
    public class Map
    {
        private readonly object _lock = new();

        [JsonProperty]
        private readonly MapTile[] _gridMap;

        /// <summary>
        /// Map all the properties of the floor based on an arbitrary enum (defined by a game implementation).
        /// </summary>
        [JsonProperty]
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

        public readonly int Width;
        public readonly int Height;

        [JsonConstructor]
        public Map(int width, int height)
        {
            (Width, Height) = (width, height);

            _gridMap = new MapTile[width * height];
            _floorMap = new int[width * height];

            Array.Fill(_gridMap, new());
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
        public bool HasLineOfSight(Point start, Point end, bool excludeEdges, int blocking = CollisionLayersBase.SOLID)
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
        private bool HasLineOfSight(Point start, Point end, bool excludeEdges, Func<int, int, bool> filter)
        {
            var line = GridHelper.Line(start, end.Clamp(0, 0, Width, Height)).ToImmutableArray(); // Eeeehh I don't like this
            bool isWall = GetCollision(end.X, end.Y).HasFlag(CollisionLayersBase.SOLID);

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

        //public bool HasAnyCollision(int x, int y)
        //    => HasCollision(x, y, GridCollisionType.Static | GridCollisionType.Carve);

        //public bool HasCarveCollision(int x, int y)
        //    => HasCollision(x, y, GridCollisionType.Carve);

        public bool HasCollision(int x, int y, int layer)
        {
            return (GetCollision(x, y) & layer) != CollisionLayersBase.NONE;
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

        public int GetCollision(int x, int y)
        {
            if (!IsInsideGrid(x, y))
                return CollisionLayersBase.SOLID;

            return At(x, y);
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

        public void SetOccupiedAsStatic(int x, int y, int layer)
        {
            _gridMap[(y * Width) + x].CollisionType |= layer;
        }

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

            for (int cy = y; cy < y + height; cy++)
            {
                for (int cx = x; cx < x + width; cx++)
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

        private int At(int x, int y)
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