using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Helpers;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Services;

public static class EntityServices
{
    public static Vector2 FetchScale(this Entity entity)
    {
        return entity.TryGetScale()?.Scale ?? Vector2.One;
    }
    public static Vector2? GetGlobalPositionIfValid(this Entity? entity)
    {
        if (entity is null || !entity.IsActive || entity.IsDestroyed || !entity.HasTransform())
        {
            return null;
        }
        
        return entity.GetGlobalTransform().Vector2;
    }

    public static void SubscribeToAnimationEvents(this Entity listener, Entity broadcaster)
    {
        if (broadcaster.TryGetAnimationEventBroadcaster() is not AnimationEventBroadcasterComponent broadcasterComponent)
        {
            broadcasterComponent = new();
        }

        broadcaster.SetAnimationEventBroadcaster(broadcasterComponent.Subscribe(listener.EntityId));
    }

    public static void RotateChildPositions(World world, Entity entity, float angle)
    {
        foreach (var id in entity.Children)
        {
            if (world.TryGetEntity(id) is Entity child)
            {
                Vector2 localPosition = child.GetMurderTransform().Vector2;
                child.SetTransform(new PositionComponent(localPosition.Rotate(angle)));
            }
        }
    }

    public static void RotatePositionAround(Entity entity, Vector2 center, float angle)
    {
        Vector2 localPosition = entity.GetMurderTransform().Vector2 - center;
        entity.SetTransform(new PositionComponent(center + localPosition.Rotate(angle)));
    }

    public static void RotatePosition(Entity entity, float angle)
    {
        Vector2 localPosition = entity.GetMurderTransform().Vector2;
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

    public static SpriteComponent? PlaySpriteAnimationNext(this Entity entity, string animationName)
    {
        if (TryPlaySpriteAnimationNext(entity, animationName) is SpriteComponent result)
        {
            return result;
        }

        GameLogger.Error($"Entity {entity.EntityId} doesn's have an Seprite component ({entity.Components.Count()} components, trying to play '{animationName}')");
        return null;
    }

    public static SpriteComponent? TryPlaySpriteAnimationNext(this Entity entity, string animationName)
    {
        if (entity.TryGetSprite() is SpriteComponent aseprite)
        {
            SpriteComponent result = aseprite.PlayAfter(animationName);
            entity.SetSprite(result);

            entity.RemoveAnimationComplete();
            entity.RemoveAnimationCompleteMessage();

            return result;
        }

        return null;
    }

    public static SpriteComponent? PlaySpriteAnimationNext(this Entity entity, params string[] animations)
    {
        if (entity.TryGetSprite() is SpriteComponent aseprite)
        {
            SpriteComponent result = aseprite.PlayAfter(animations);
            entity.SetSprite(result);

            entity.RemoveAnimationComplete();
            entity.RemoveAnimationCompleteMessage();

            return result;
        }

        return null;
    }

    public static SpriteComponent? PlaySpriteAnimationNext(this Entity entity, ImmutableArray<string> animations)
    {
        if (entity.TryGetSprite() is SpriteComponent aseprite)
        {
            SpriteComponent result = aseprite.PlayAfter(animations);
            entity.SetSprite(result);

            entity.RemoveAnimationComplete();
            entity.RemoveAnimationCompleteMessage();

            return result;
        }

        return null;
    }

    public static SpriteComponent? TryPlaySpriteAnimation(this Entity entity, params string[] nextAnimations) =>
        TryPlaySpriteAnimation(entity, nextAnimations.ToImmutableArray());

    public static SpriteComponent? TryPlaySpriteAnimation(this Entity entity, ImmutableArray<string> nextAnimations, Guid? replaceSpriteGuid = null)
    {
        if (entity.TryGetSprite() is SpriteComponent sprite)
        {
            entity.RemoveDoNotLoop();
            entity.RemoveAnimationStarted();
            entity.RemoveAnimationComplete();
            entity.RemoveAnimationCompleteMessage();

            if (sprite.IsPlaying(nextAnimations))
            {
                return sprite;
            }

            SpriteComponent result;
            if (nextAnimations.Length == 0)
            {
                result = sprite.Play(sprite.NextAnimations, replaceSpriteGuid);
            }
            else
            {
                result = sprite.Play(nextAnimations, replaceSpriteGuid);
            }

            entity.SetSprite(result);

            float startedTime = Game.Now;
            if (entity.TryGetRandomizeSprite() is RandomizeSpriteComponent randomize && randomize.RandomizeAnimationStart)
            {
                startedTime = Game.Random.NextFloat(1f, 32f);
            }

            entity.SetAnimationStarted(startedTime);
            return result;
        }

        return null;
    }

    /// <summary>
    /// Plays an animation or animation sequence. Loops the last animation.
    /// </summary>
    public static SpriteComponent? PlaySpriteAnimation(this Entity entity, ImmutableArray<string> animations, Guid? replaceSpriteGuid = null)
    {
        if (TryPlaySpriteAnimation(entity, animations, replaceSpriteGuid) is SpriteComponent aseprite)
        {
            return aseprite;
        }

        GameLogger.Error($"Entity {entity.EntityId} doesn's have an Sprite component ({entity.Components.Count()} components, trying to play '{string.Join(',', animations)}')");
        return null;
    }

    /// <summary>
    /// Plays an animation or animation sequence. Loops the last animation.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="nextAnimations"></param>
    /// <returns></returns>
    public static SpriteComponent? PlaySpriteAnimation(this Entity entity, params string[] nextAnimations)
    {
        if (TryPlaySpriteAnimation(entity, nextAnimations) is SpriteComponent aseprite)
        {
            return aseprite;
        }

        GameLogger.Error($"Entity {entity.EntityId} doesn's have an Sprite component ({entity.Components.Count()} components, trying to play '{string.Join(',', nextAnimations)}')");
        return null;
    }

    public static void RestartSpriteAnimation(this Entity entity)
    {
        entity.SetAnimationStarted(Game.Now);
    }

    public static bool AnimationAvailable(this Entity entity, string id)
    {
        if (entity.TryGetAgentSprite() is AgentSpriteComponent agentSprite)
        {
            var sprite = Game.Data.TryGetAsset<SpriteAsset>(agentSprite.AnimationGuid);
            if (sprite != null)
            {
                if (sprite.Animations.ContainsKey(id))
                {
                    return true;
                }
                else if (entity.TryGetFacing() is FacingComponent facing)
                {
                    (string suffix, bool flip) = DirectionHelper.GetSuffixFromAngle(entity, id, facing.Angle);
                    if (sprite.Animations.ContainsKey($"{id}_{suffix}"))
                        return true;
                }
            }
        }
        else if (entity.TryGetSprite() is SpriteComponent basicSprite)
        {
            var sprite = Game.Data.TryGetAsset<SpriteAsset>(basicSprite.AnimationGuid);
            if (sprite != null)
            {
                return sprite.Animations.ContainsKey(id);
            }
        }

        return false;
    }

    public static SpriteAsset? TryActiveSpriteAsset(this Entity entity)
    {
        if (entity.TryGetAgentSprite() is AgentSpriteComponent agentSprite)
        {
            return Game.Data.TryGetAsset<SpriteAsset>(agentSprite.AnimationGuid);
        }
        else if (entity.TryGetSprite() is SpriteComponent sprite)
        {
            return Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid);
        }

        return null;
    }

    public static void Spawn(World world, Vector2 spawnerPosition, Guid entityToSpawn, int count, float radius, params IComponent[] addComponents)
    {
        Vector2 tentativePosition = Calculator.RandomPointInCircleEdge() * radius;

        for (int i = 0; i < count; i++)
        {
            if (AssetServices.TryCreate(world, entityToSpawn) is Entity spawned)
            {
                Vector2 position = Point.Zero;
                if (PhysicsServices.FindNextAvailablePosition(world, spawned, spawnerPosition, spawnerPosition + tentativePosition, CollisionLayersBase.SOLID | CollisionLayersBase.HOLE | CollisionLayersBase.ACTOR) is Vector2 targetGlobalPosition)
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

    /// <summary>
    /// Plays an animation or animation sequence. Do not loop.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="animation"></param>
    /// <returns></returns>
    public static SpriteComponent? PlaySpriteAnimationOnce(this Entity entity, string animation)
    {
        if (entity.TryGetSprite() is SpriteComponent sprite)
        {
            SpriteComponent result = sprite.Play(animation);
            entity.SetSprite(result);

            entity.SetDoNotLoop();

            entity.RemoveAnimationCompleteMessage();
            entity.RemoveAnimationComplete();
            entity.RemoveAnimationStarted();

            return result;
        }

        GameLogger.Error($"Entity {entity.EntityId} doesn't have a sprite component");
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

    /// <summary>
    /// Return all targets of entity that start with <paramref name="prefix"/>.
    /// This will first check for a target id. If none, it will check for a target
    /// collection with <paramref name="prefix"/>.
    /// </summary>
    public static IEnumerable<int> FindAllTargets(this Entity e, string prefix)
    {
        if (e.TryGetIdTarget()?.Target is int targetId)
        {
            yield return targetId;
        }

        if (e.TryGetIdTargetCollection()?.Targets is ImmutableDictionary<string, int> targets)
        {
            foreach (var (key, id) in targets)
            {
                if (key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return id;
                }
            }
        }
    }

    public static bool IsInCamera(this Entity e, World world)
    {
        if (!e.HasTransform())
        {
            // No transform? Assume it's ~*everywhere~*.
            return true;
        }

        Point p = e.GetGlobalTransform().Point;

        return ((MonoWorld)world).Camera.Bounds.Contains(p);
    }

    public static void RemoveSpeedMultiplier(Entity entity, int slot)
    {
        SetAgentSpeedMultiplier(entity, slot, 1f);
    }

    public static void SetAgentSpeedMultiplier(Entity entity, int slot, float speedMultiplier)
    {
        if (entity.TryGetAgentSpeedMultiplier() is AgentSpeedMultiplierComponent agentSpeedMultiplier)
        {
            entity.SetAgentSpeedMultiplier(new AgentSpeedMultiplierComponent()
            {
                SpeedMultiplier = agentSpeedMultiplier.SpeedMultiplier.SetItem(slot, speedMultiplier)
            });
        }
        else
        {
            entity.SetAgentSpeedMultiplier(slot, speedMultiplier);
        }
    }
    public static string? TryGetEntityName(Entity entity)
    {
        if (entity.TryGetComponent<PrefabRefComponent>(out var assetComponent))
        {
            if (Game.Data.TryGetAsset<PrefabAsset>(assetComponent.AssetGuid) is PrefabAsset asset)
            {
                return asset.Name;
            }
        }

        return null;
    }

    /// <summary>
    /// Return target of entity with <paramref name="name"/>.
    /// This will first check for a target id. If none, it will check for a target
    /// collection with <paramref name="name"/>.
    /// </summary>
    public static int? FindTarget(this Entity e, string name)
    {
        if (!string.IsNullOrEmpty(name) &&
            (e.TryGetIdTargetCollection()?.Targets?.TryGetValue(name, out int id) ?? false))
        {
            return id;
        }

        if (e.TryGetIdTarget()?.Target is int targetId)
        {
            return targetId;
        }

        return null;
    }

    /// <summary>
    /// Return target of entity with <paramref name="name"/>.
    /// This will first check for a target id. If none, it will check for a target
    /// collection with <paramref name="name"/>.
    /// Returns the entity in the world.
    /// </summary>
    public static Entity? FetchTarget(this Entity e, World world, string name)
    {
        int? id = FindTarget(e, name);
        if (id is null)
        {
            return null;
        }

        return world.TryGetEntity(id.Value);
    }

    public static void PauseAgent(Entity e)
    {
        if (e.TryGetAgentPause() is AgentPauseComponent agentPause)
        {
            agentPause = agentPause.Add();
        }
        else
        {
            agentPause = new();
        }

        e.SetAgentPause(agentPause);
    }

    public static void UnpauseAgent(Entity e)
    {
        if (e.TryGetAgentPause() is not AgentPauseComponent agentPause)
        {
            return;
        }

        agentPause = agentPause.Remove();
        if (agentPause.Count == 0)
        {
            e.RemoveAgentPause();
        }
        else
        {
            e.SetAgentPause(agentPause);
        }
    }

    public static void PlayAnimationOverload(
        this Entity e, 
        string animation, 
        AnimationOverloadProperties properties = AnimationOverloadProperties.Loop | AnimationOverloadProperties.IgnoreFacing, 
        int offset = 0,
        Guid? customSprite = null)
    {
        ImmutableArray<string> animations;
        if (properties.HasFlag(AnimationOverloadProperties.Disappear))
        {
            animations = [animation, "_"];
        }
        else
        {
            animations = [animation];
        }

        PlayAnimationOverload(e, animations, properties, offset, customSprite);
    }

    public static void PlayAnimationOverload(
        this Entity e,
        ImmutableArray<string> animations,
        AnimationOverloadProperties properties = AnimationOverloadProperties.Loop | AnimationOverloadProperties.IgnoreFacing,
        int offset = 0,
        Guid? customSprite = null)
    {
        AnimationOverloadComponent overload =
                    new AnimationOverloadComponent(
                        animations,
                        loop: properties.HasFlag(AnimationOverloadProperties.Loop),
                        ignoreFacing: properties.HasFlag(AnimationOverloadProperties.IgnoreFacing),
                        customSprite: customSprite ?? Guid.Empty)
                    with
                    { 
                        SortOffset = offset,
                        Flip = properties.HasFlag(AnimationOverloadProperties.FlipHorizontal) ? 
                                ImageFlip.Horizontal : ImageFlip.None,
                        SupportedDirections = properties.HasFlag(AnimationOverloadProperties.LockTo4Directions) ? 
                                4 : null
                    };

        e.SetAnimationOverload(overload);
        e.RemoveAnimationComplete();
        e.RemoveAnimationCompleteMessage();
    }
}

[Flags]
public enum AnimationOverloadProperties
{
    None = 0,
    Loop = 0x1,
    IgnoreFacing = 0x10,
    FlipHorizontal = 0x100,
    LockTo4Directions = 0x1000,
    /// <summary>
    /// Not implemented yet.
    /// </summary>
    Once = 0x10000,
    Disappear = Once | 0x100000
}