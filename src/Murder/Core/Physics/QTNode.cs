// Code based on https://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374

using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Physics
{
    public readonly struct NodeInfo<T> where T : notnull
    {
        public readonly T EntityInfo;
        public readonly Rectangle BoundingBox;

        public NodeInfo(T info, Rectangle boundingBox)
        {
            EntityInfo = info;
            BoundingBox = boundingBox;
        }
    }

    public class QTNode<T> where T : notnull
    {
        private const int MAX_OBJECTS = 6;
        private const int MAX_LEVELS = 6;

        public ImmutableArray<QTNode<T>> Nodes = ImmutableArray.Create<QTNode<T>>();
        public readonly Rectangle Bounds;
        public readonly int Level;

        /// <summary>
        /// Entities are indexed by their entity ID number
        /// </summary>
        public readonly Dictionary<int, NodeInfo<T>> Entities = new(MAX_OBJECTS);


        public QTNode(int level, Rectangle bounds)
        {
            Bounds = bounds;
            Level = level;
        }

        public void DrawDebug(Batch2D spriteBatch)
        {
            var depthColor = new Color(1 - 1f / (1f + Level), 1f / (1f + Level), 2 -  2f / (2f + Level));
            var bounds = Bounds;

            spriteBatch.DrawRectangleOutline(bounds, depthColor);
            RenderServices.DrawText(spriteBatch, MurderFonts.PixelFont, Entities.Count.ToString(), bounds.TopLeft.Point + new Point(2, 2) * (1 + Level),
                new DrawInfo(0)
                {
                    Color = depthColor
                });

            if (!Nodes.IsDefaultOrEmpty)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i].DrawDebug(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Recursively clears all entities of the node, but keeps the structure
        /// </summary>
        public void Clear()
        {
            Entities.Clear();
            Nodes = ImmutableArray.Create<QTNode<T>>();
            if (!Nodes.IsDefaultOrEmpty)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    Nodes[i].Clear();
                }
            }
        }

        /// <summary>
        /// Completely resets the node removing anything inside
        /// </summary>
        public void Reset()
        {
            Entities.Clear();
            Nodes = ImmutableArray.Create<QTNode<T>>();
        }

        public void Split()
        {
            var builder = ImmutableArray.CreateBuilder<QTNode<T>>(4);
            int subWidth = Calculator.RoundToInt(Bounds.Width / 2);
            int subHeight = Calculator.RoundToInt(Bounds.Height / 2);
            int x = Calculator.RoundToInt(Bounds.X);
            int y = Calculator.RoundToInt(Bounds.Y);

            builder.Add(new QTNode<T>(Level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight)));
            builder.Add(new QTNode<T>(Level + 1, new Rectangle(x, y, subWidth, subHeight)));
            builder.Add(new QTNode<T>(Level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight)));
            builder.Add(new QTNode<T>(Level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight)));
        
            Nodes = builder.ToImmutableArray();
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
            float verticalMidpoint = Bounds.X + (Bounds.Width / 2f);
            float horizontalMidpoint = Bounds.Y + (Bounds.Height / 2f);

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
            bool success = false;
            if (Entities.Remove(entityId))
            {
                success = true;
            }

            if (!Nodes.IsDefaultOrEmpty)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].Remove(entityId))
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
            if (!Nodes.IsDefaultOrEmpty)
            {
                int index = GetIndex(boundingBox);

                if (index != -1)
                {
                    Nodes[index].Insert(entityId, info, boundingBox);

                    return;
                }
            }

            Entities[entityId] = new NodeInfo<T>(info, boundingBox);

            if (Entities.Count > MAX_OBJECTS && Level < MAX_LEVELS)
            {
                if (Nodes.IsDefaultOrEmpty)
                {
                    Split();
                }
                
                foreach (var e in Entities)
                {
                    var bb = e.Value.BoundingBox;
                    int index = GetIndex(bb);
                    if (index != -1)
                    {
                        Nodes[index].Insert(e.Key, e.Value.EntityInfo, bb);
                        Entities.Remove(e.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Return all objects that could collide with the given object at <paramref name="returnEntities"/>.
        /// </summary>
        public void Retrieve(Rectangle boundingBox, List<NodeInfo<T>> returnEntities)
        {
            if (!Nodes.IsDefaultOrEmpty)
            {
                int index = GetIndex(boundingBox);
                if (index != -1) // This bounding box can be contained inside a single node
                {
                    Nodes[index].Retrieve(boundingBox, returnEntities);
                }
                else
                {
                    for (int i = 0; i <= 3; i++) // The Bounding box is to big to be contained by a single node
                    {
                        Nodes[i].Retrieve(boundingBox, returnEntities);
                    }
                }
            }

            foreach (var item in Entities)
            {
                returnEntities.Add(item.Value);
            }
        }
    }
}
