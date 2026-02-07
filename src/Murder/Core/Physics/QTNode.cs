// Code based on https://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374

using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Murder.Core.Physics
{
    public readonly struct NodeInfo<T> where T : notnull
    {
        public readonly int Id;
        public readonly T EntityInfo;
        public readonly Rectangle BoundingBox;

        public NodeInfo(int id, T info, IntRectangle boundingBox)
        {
            Id = id;
            EntityInfo = info;
            BoundingBox = boundingBox;
        }
    }

    public class QTNode<T> where T : notnull
    {
        private const int MAX_OBJECTS = 32;
        private const int MAX_LEVELS = 6;


        // Children don't exist until split, then always length 4
        private QTNode<T>[]? _nodes;
        private readonly IntRectangle _bounds;
        private readonly int _level;

        /// <summary>
        /// Entities are indexed by their entity ID number
        /// </summary>
        public readonly Dictionary<int, NodeInfo<T>> _entities = new(MAX_OBJECTS);

        // entity ID -> which node contains it (shared across all nodes)
        private readonly Dictionary<int, QTNode<T>>? _entityLookup;

        // Root constructor
        public QTNode(int level, IntRectangle bounds) : this(level, bounds, new Dictionary<int, QTNode<T>>(256)) { }

        // Child constructor
        private QTNode(int level, IntRectangle bounds, Dictionary<int, QTNode<T>> entityLookup)
        {
            _bounds = bounds;
            _level = level;
            _entityLookup = level == 0 ? entityLookup : null; // Only root owns lookup
        }

        public void DrawDebug(Batch2D spriteBatch)
        {
            var depthColor = new Color(1 - 1f / (1f + _level), 1f / (1f + _level), 2 - 2f / (2f + _level));
            var bounds = _bounds;

            spriteBatch.DrawRectangleOutline(bounds, depthColor);
            RenderServices.DrawText(spriteBatch, MurderFonts.PixelFont, _entities.Count.ToString(), bounds.TopLeft + new Point(4, 4) * (1 + _level),
                new DrawInfo(0)
                {
                    Color = depthColor
                });

            if (_nodes is not null)
            {
                for (int i = 0; i < _nodes.Length; i++)
                {
                    _nodes[i].DrawDebug(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Recursively clears all entities of the node, but keeps the structure
        /// </summary>
        public void Clear()
        {
            _entities.Clear();

            if (_nodes is not null)
            {
                for (int i = 0; i < 4; i++)
                {
                    _nodes[i].Clear();
                }
            }

            _entityLookup?.Clear();
        }

        /// <summary>
        /// Completely resets the node removing anything inside
        /// </summary>
        public void Reset()
        {
            _entities.Clear();
            _nodes = null;
            _entityLookup?.Clear();
        }

        private void Split()
        {
            int subWidth = _bounds.Width / 2;
            int subHeight = _bounds.Height / 2;
            int x = _bounds.X;
            int y = _bounds.Y;

            _nodes =
            [
                new QTNode<T>(_level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight)),
                new QTNode<T>(_level + 1, new Rectangle(x, y, subWidth, subHeight)),
                new QTNode<T>(_level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight)),
                new QTNode<T>(_level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight))
            ];
        }

        /// <summary>
        /// Determine which node the object belongs to. -1 means
        /// object cannot completely fit within a child node and is part
        /// of the parent node
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <returns></returns>
        private int GetIndex(Rectangle boundingBox)
        {
            int index = -1;
            float verticalMidpoint = _bounds.X + (_bounds.Width / 2f);
            float horizontalMidpoint = _bounds.Y + (_bounds.Height / 2f);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (boundingBox.Top < horizontalMidpoint && boundingBox.Bottom < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (boundingBox.Top > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (boundingBox.Left < verticalMidpoint && boundingBox.Right < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants
            else if (boundingBox.Left > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }

        /// <summary>
        /// Removes an entity from the quadtree
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public bool Remove(int entityId)
        {
            if (_entityLookup is not null && _entityLookup.TryGetValue(entityId, out var node))
            {
                node._entities.Remove(entityId);
                _entityLookup.Remove(entityId);
                return true;
            }
            // fallback for non-root calls (shouldn't happen if you always call from root)
            return RemoveSlow(entityId);
        }


        private bool RemoveSlow(int entityId)
        {
            bool success = false;
            if (_entities.Remove(entityId))
            {
                success = true;
            }

            if (_nodes is not null)
            {
                for (int i = 0; i < _nodes.Length; i++)
                {
                    if (_nodes[i].Remove(entityId))
                    {
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Insert the object into the quadtree. If the node
        /// exceeds the capacity, it will split and add all
        /// objects to their corresponding nodes.
        /// </summary>
        public void Insert(int entityId, T info, Rectangle boundingBox)
        {
            InsertInternal(entityId, info, boundingBox, _entityLookup);
        }

        private void InsertInternal(int entityId, T info, Rectangle boundingBox, Dictionary<int, QTNode<T>>? lookup)
        {
            // Try to insert into child node
            if (_nodes is not null)
            {
                int index = GetIndex(boundingBox);
                if (index != -1)
                {
                    _nodes[index].InsertInternal(entityId, info, boundingBox, lookup);
                    return;
                }
            }

            // Doesn't fit inside a child or no children exist
            // Let's add it here
            _entities[entityId] = new NodeInfo<T>(entityId, info, boundingBox);
            lookup?.TryAdd(entityId, this);

            // Split if needed
            if (_entities.Count > MAX_OBJECTS && _level < MAX_LEVELS && _nodes is null)
            {
                Split();

                // Can I give any of my entities to my children?
                // If so share them downwards!
                Span<int> keysToMove = stackalloc int[_entities.Count];
                int moveCount = 0;

                foreach (var kvp in _entities)
                {
                    int index = GetIndex(kvp.Value.BoundingBox);
                    if (index != -1)
                    {
                        keysToMove[moveCount++] = kvp.Key;
                    }
                }

                for (int i = 0; i < moveCount; i++)
                {
                    int key = keysToMove[i];
                    var nodeInfo = _entities[key];
                    int index = GetIndex(nodeInfo.BoundingBox);

                    _entities.Remove(key);

                    Debug.Assert(_nodes is not null, "This should never be null here!");
                    _nodes[index].InsertInternal(key, nodeInfo.EntityInfo, nodeInfo.BoundingBox, lookup);
                }
            }
        }
        public void Update(int entityId, T info, Rectangle newBounds)
        {
            if (_entityLookup is not null && _entityLookup.TryGetValue(entityId, out var currentNode))
            {
                // Check if entity still fits in current node
                if (currentNode.GetIndex(newBounds) == -1 && currentNode._bounds.Contains(newBounds))
                {
                    // Still fits, just update in place
                    currentNode._entities[entityId] = new NodeInfo<T>(entityId, info, newBounds);
                    return;
                }

                // Needs to move
                currentNode._entities.Remove(entityId);
                _entityLookup.Remove(entityId);
            }

            Insert(entityId, info, newBounds);
        }

        /// <summary>
        /// Return all objects that could collide with the given object at <paramref name="results"/>.
        /// </summary>
        public void Retrieve(Rectangle boundingBox, List<NodeInfo<T>> results)
        {
            // Add entities from this node that actually overlap
            foreach (var kvp in _entities)
            {
                if (kvp.Value.BoundingBox.Touches(boundingBox))
                {
                    results.Add(kvp.Value);
                }
            }

            if (_nodes is null)
                return;

            int index = GetIndex(boundingBox);
            if (index != -1)
            {
                // Query fits in single child
                _nodes[index].Retrieve(boundingBox, results);
            }
            else
            {
                // Query spans multiple children, check all that overlap
                for (int i = 0; i < 4; i++)
                {
                    if (_nodes[i]._bounds.Touches(boundingBox))
                    {
                        _nodes[i].Retrieve(boundingBox, results);
                    }
                }
            }
        }
    }
}