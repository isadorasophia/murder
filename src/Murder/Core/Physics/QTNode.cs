// Code based on https://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374

using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Physics
{
    public class QTNode<T>
    {
        private const int MAX_OBJECTS = 6;
        private const int MAX_LEVELS = 6;

        public ImmutableArray<QTNode<T>> Nodes = ImmutableArray.Create<QTNode<T>>();
        public readonly Rectangle Bounds;
        public readonly int Level;
        public readonly List<(T entity, Rectangle boudingBox)> Entities = new(MAX_OBJECTS);

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
        /// Recursivelly clears all entities of the node, but keeps the strtucture
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
        /// Completelly resets the node removing anything inside
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
            double verticalMidpoint = Bounds.X + Calculator.RoundToInt(Bounds.Width / 2);
            double horizontalMidpoint = Bounds.Y + Calculator.RoundToInt(Bounds.Height / 2);

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
        /// Insert the object into the quadtree. If the node
        /// exceeds the capacity, it will split and add all
        /// objects to their corresponding nodes.
        /// </summary>
        public void Insert(T entity, Rectangle boundingBox)
        {
            if (!Nodes.IsDefaultOrEmpty)
            {
                int index = GetIndex(boundingBox);

                if (index != -1)
                {
                    Nodes[index].Insert(entity, boundingBox);

                    return;
                }
            }
        
            Entities.Add((entity, boundingBox));

            if (Entities.Count > MAX_OBJECTS && Level < MAX_LEVELS)
            {
                if (Nodes.IsDefaultOrEmpty)
                {
                    Split();
                }

                int i = 0;
                while (i < Entities.Count)
                {
                    var e = Entities[i];
                    var bb = e.boudingBox;
                    int index = GetIndex(bb);
                    if (index != -1)
                    {
                        Nodes[index].Insert(e.entity, bb);
                        Entities.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Return all objects that could collide with the given object at <paramref name="returnEntities"/>.
        /// </summary>
        public void Retrieve(Rectangle boundingBox, ref List<(T entity, Rectangle boundingBox)> returnEntities)
        {
            if (!Nodes.IsDefaultOrEmpty)
            {
                int index = GetIndex(boundingBox);
                if (index != -1) // This bounding box can be contained inside a single node
                {
                    Nodes[index].Retrieve(boundingBox, ref returnEntities);
                }
                else
                {
                    for (int i = 0; i <= 3; i++) // The Bounding box is to big to be contained by a single node
                    {
                        Nodes[i].Retrieve(boundingBox, ref returnEntities);
                    }
                }
            }

            returnEntities.AddRange(Entities);
        }
    }
}
