using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Services
{
    public static class EntityServices
    {
        public static void RotateChildPositions(World world, Entity entity, float angle)
        {
            foreach (var id in entity.Children)
            {
                if (world.TryGetEntity(id) is Entity child)
                {
                    Vector2 localPosition = child.GetTransform().Vector2;
                    child.SetTransform(new PositionComponent(localPosition.Rotate(angle)));
                }
            }
        }
        
        public static void RotatePositionAround(Entity entity, Vector2 center, float angle)
        {
            Vector2 localPosition = entity.GetTransform().Vector2 - center;
            entity.SetTransform(new PositionComponent(center + localPosition.Rotate(angle)));
        }
        
        public static void RotatePosition(Entity entity, float angle)
        {
            Vector2 localPosition = entity.GetTransform().Vector2;
            entity.SetTransform(new PositionComponent(localPosition.Rotate(angle)));
        }

        public static bool IsChildOf(World world, Entity parent, Entity child)
        {
            if (child.Parent.HasValue)
            {
                if (child.Parent.Value == parent.EntityId)
                {
                    return true;
                }
                else
                {
                    if (world.TryGetEntity(child.Parent.Value) is Entity parentEntity)
                    {
                        return IsChildOf(world, parent, parentEntity);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Recursivelly get all children of this entity, including itself.
        /// </summary>
        /// <param name="world">World that this entity belongs.</param>
        /// <param name="entity">Entity target.</param>
        internal static IEnumerable<int> GetAllTreeOfEntities(World world, Entity entity)
        {
            // First, return ourselves.
            yield return entity.EntityId;

            foreach (int child in GetAllChildren(world, entity))
            {
                yield return child;
            }
        }
        
        internal static IEnumerable<int> GetAllChildren(World world, Entity entity)
        {
            foreach (int childId in entity.Children)
            {
                if (world.TryGetEntity(childId) is not Entity child)
                {
                    // Child died...?
                    continue;
                }
                
                // Return the child and its children
                foreach (int entityIdInTree in GetAllTreeOfEntities(world, child))
                {
                    yield return entityIdInTree;
                }
            }
        }

        public static Entity FindRootEntity(Entity e)
        {
            if (e.Parent is null || e.TryFetchParent() is not Entity parent)
            {
                return e;
            }

            return FindRootEntity(parent);
        }

        public static SpriteComponent? TryPlayAsepriteAnimationNext(this Entity entity, string animationName)
        {

            if (entity.TryGetSprite() is SpriteComponent aseprite)
            {
                SpriteComponent result = aseprite.PlayAfter(animationName);
                entity.SetSprite(result);
                entity.RemoveAnimationComplete();

                return result;
            }

            return null;
        }
        public static SpriteComponent? PlayAsepriteAnimationNext(this Entity entity, string animationName)
        {
            if (TryPlayAsepriteAnimationNext(entity, animationName) is SpriteComponent result)
            {
                return result;
            }

            GameLogger.Error("Entity doesn's have an Aseprite component");
            return null;
        }

        public static SpriteComponent? TryPlayAsepriteAnimation(this Entity entity, params string[] nextAnimations)
        {
            if (entity.TryGetSprite() is SpriteComponent aseprite)
            {
                if (aseprite.IsPlaying(nextAnimations) || nextAnimations.Length == 0)
                    return aseprite;

                SpriteComponent result = aseprite.Play(!entity.HasPauseAnimation(), nextAnimations);
                entity.SetSprite(result);
                entity.RemoveAnimationComplete();

                return result;
            }

            return null;
        }

        public static SpriteComponent? PlaySpriteAnimation(this Entity entity, params string[] nextAnimations)
        {
            if (TryPlayAsepriteAnimation(entity, nextAnimations) is SpriteComponent aseprite)
            {
                return aseprite;
            }

            GameLogger.Error("Entity doesn's have an Aseprite component");
            return null;
        }

        public static void Spawn(World world, Vector2 spawnerPosition, Guid entityToSpawn, int count, float radius = Grid.CellSize, params IComponent[] addComponents)
        {
            Vector2 tentativePosition = Calculator.RandomPointInCircleEdge() * radius;

            for (int i = 0; i < count; i++)
            {
                if (AssetServices.TryCreate(world, entityToSpawn) is Entity spawned)
                {
                    Vector2 position = Point.Zero;
                    if (PhysicsServices.FindNextAvailablePosition(world, spawned, spawnerPosition + tentativePosition) is Vector2 targetGlobalPosition)
                    {
                        position = targetGlobalPosition;
                    }

                    spawned.SetTransform(new PositionComponent(position));
                    foreach (var c in addComponents)
                    {
                        spawned.AddComponent(c);
                    }
                }
            }
        }

        public static SpriteComponent? PlayAsepriteAnimation(this Entity entity, ImmutableArray<string> animations)
        {
            if (entity.TryGetSprite() is SpriteComponent aseprite)
            {
                SpriteComponent result = aseprite.Play(!entity.HasPauseAnimation(), animations);
                entity.SetSprite(result);
                entity.RemoveAnimationComplete();
                
                return result;
            }

            GameLogger.Error("Entity doesn't have an Aseprite component");
            return null;
        }

        /// <summary>
        /// Try to find the target of a <see cref="GuidToIdTargetComponent"/>.
        /// </summary>
        public static Entity? TryFindTarget(this Entity entity, World world)
        {
            if (entity.TryGetIdTarget()?.Target is not int id)
            {
                return default;
            }

            return world.TryGetEntity(id);
        }

        /// <summary>
        /// Try to find the target of a <see cref="GuidToIdTargetCollectionComponent"/>.
        /// </summary>
        public static Entity? TryFindTarget(this Entity entity, World world, string name)
        {
            if (entity.TryGetIdTargetCollection()?.Targets is not ImmutableDictionary<string, int> targets
                || !targets.TryGetValue(name, out int id))
            {
                return default;
            }

            return world.TryGetEntity(id);
        }

        public static bool IsInCamera(this Entity e, World world)
        {
            Point p = e.GetGlobalTransform().Point;

            return ((MonoWorld)world).Camera.SafeBounds.Contains(p);
        }
    }
}
