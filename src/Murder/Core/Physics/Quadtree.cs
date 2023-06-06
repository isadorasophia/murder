using Newtonsoft.Json;
using Murder.Core.Geometry;
using Bang.Entities;
using Murder.Components;
using Murder.Services;
using Murder.Utilities;
using Murder.Assets.Graphics;
using Bang;
using Bang.Contexts;
using Murder.Diagnostics;

namespace Murder.Core.Physics
{
    public class Quadtree
    {
        [JsonIgnore]
        public readonly QTNode<Entity> Collision;

        [JsonIgnore]
        public readonly QTNode<(
            Entity entity,
            IMurderTransformComponent position,
            PushAwayComponent pushAway,
            Vector2 velocity
            )> PushAway;

        [JsonIgnore]
        public readonly QTNode<(Entity entity, SpriteComponent sprite, Vector2 renderPosition)> StaticRender;
        
        public Quadtree(Rectangle mapBounds)
        {
            Collision = new(0, mapBounds);
            PushAway = new(0, mapBounds);
            StaticRender = new(0, mapBounds);
        }

        public void AddToCollisionQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                IMurderTransformComponent pos = e.GetGlobalTransform();
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    Collision.Insert(e, collider.GetBoundingBox(pos.Point));
                }
            }
        }

        public void RemoveFromCollisionQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (!Collision.Remove(e))
                {
                    // This is actually normal and can happen sometimes
                    // Maybe we can avoid it in the future though [PERF]
                    // GameLogger.Error("Failed to remove entity from quadtree");
                }
            }
        }

        /// <summary>
        /// Completelly clears and rebuilds the quad tree and pushAway quad tree using a given list of entities
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateQuadTree(IEnumerable<Entity> entities)
        {
            Collision.Clear();
            PushAway.Clear();

            foreach (var e in entities)
            {
                IMurderTransformComponent pos = e.GetGlobalTransform();
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    Collision.Insert(e, collider.GetBoundingBox(pos.Point));
                }

                if (e.TryGetPushAway() is PushAwayComponent pushAway)
                {
                    PushAway.Insert(
                        (
                            e,
                            e.GetGlobalTransform(),
                            e.GetPushAway(),
                            e.TryGetVelocity()?.Velocity ?? Vector2.Zero
                        ), new Rectangle(pos.X, pos.Y, pushAway.Size, pushAway.Size));
                }
            }
        }

        public void RemoveFromStaticRenderQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                Point position = e.GetGlobalTransform().Point;
                SpriteComponent spriteComponent = e.GetSprite();

                if (Game.Data.TryGetAsset<SpriteAsset>(spriteComponent.AnimationGuid) is not SpriteAsset spriteAsset)
                    continue;
                
                StaticRender.Remove((e, spriteComponent, position));
            }
        }

        public void AddToStaticRenderQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                Point position = e.GetGlobalTransform().Point;
                SpriteComponent spriteComponent = e.GetSprite();

                if (Game.Data.TryGetAsset<SpriteAsset>(spriteComponent.AnimationGuid) is not SpriteAsset spriteAsset)
                    continue;

                // This is a greedy size, it will include the object even if it's rotated
                float diameter = (float)Math.Sqrt(spriteAsset.Size.X * spriteAsset.Size.X + spriteAsset.Size.Y * spriteAsset.Size.Y);


                StaticRender.Insert(
                    (e, spriteComponent, position), new Rectangle(
                        position.X - spriteAsset.Origin.X - spriteComponent.Offset.X * spriteAsset.Size.X - (diameter - spriteAsset.Size.X) / 2f,
                        position.Y - spriteAsset.Origin.Y - spriteComponent.Offset.Y * spriteAsset.Size.Y - (diameter - spriteAsset.Size.Y) / 2f,
                        diameter, diameter));
            }
        }

        public void GetCollisionEntitiesAt(Rectangle boundingBox, ref List<(Entity entity, Rectangle boundingBox)> list)
        {
            Collision.Retrieve(boundingBox, ref list);
        }

        internal static Quadtree GetOrCreateUnique(World world)
        {
            if (world.TryGetUnique<QuadtreeComponent>() is not QuadtreeComponent qt)
            {
                Rectangle quadTreeSize;
                if (world.TryGetUnique<MapComponent>() is MapComponent map)
                {
                    quadTreeSize = new Rectangle(0, 0, map.Width * Grid.CellSize, map.Height * Grid.CellSize);
                }
                else
                {
                    GameLogger.Warning("No size for the map was found!");

                    // TODO: We need to have the city size too!
                    quadTreeSize = new Rectangle(0, 0, 2000, 2000);
                }

                qt = new QuadtreeComponent(quadTreeSize);
                world.AddEntity(qt);
            }

            return qt.Quadtree;
        }
    }
}
