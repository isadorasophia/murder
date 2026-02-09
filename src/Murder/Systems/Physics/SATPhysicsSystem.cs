using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Physics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using static Murder.Services.PhysicsServices;

namespace Murder.Systems.Physics;


[Filter(typeof(PositionComponent), typeof(VelocityComponent))]
public class SATPhysicsSystem : IFixedUpdateSystem
{
    private const int ExhaustLimit = 10;
    private const float MinVelocityMagnitude = 0.0001f;

    private readonly List<NodeInfo<Entity>> _entityList = new();
    private readonly HashSet<int> _ignore = new();
    private readonly HashSet<int> _hitCounter = new();

    private readonly List<PhysicEntityCachedInfo> _cachedCandidates = [];

    public void FixedUpdate(Context context)
    {
        Map map = context.World.GetUniqueMap().Map;
        Quadtree qt = Quadtree.GetOrCreateUnique(context.World);
        int minStepSize = Game.Profile.MinimumVelocityForSweep;

        _entityList.Clear();

        for (int entityIndex = 0; entityIndex < context.Entities.Length; entityIndex++)
        {
            Entity e = context.Entities[entityIndex];

            bool ignoreCollisions = false;
            bool forceRespectTiles = false;

            var collider = e.TryGetCollider();
            _ignore.Clear();
            _ignore.Add(e.EntityId);
            if (e.TryGetIgnoreCollisionsWith() is IgnoreCollisionsWithComponent ignoreCollisionsWith)
            {
                foreach (var id in ignoreCollisionsWith.Ids)
                {
                    _ignore.Add(id);
                    if (context.World.TryGetEntity(id) is Entity entity)
                    {
                        foreach (var child in entity.Children)
                        {
                            _ignore.Add(child);
                        }
                    }
                }
            }

            if (e.Parent is not null)
                _ignore.Add(e.Parent.Value);

            foreach (var child in e.Children)
            {
                _ignore.Add(child);
            }

            if (e.HasAgentPause())
            {
                continue;
            }

            int mask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;
            if (e.TryGetCustomCollisionMask() is CustomCollisionMask agent)
                mask = agent.CollisionMask;

            if (e.TryGetFreeMovement() is FreeMovementComponent freeMovement)
            {
                ignoreCollisions = true;

                if (freeMovement.Flags.HasFlag(FreeMovementFlags.RespectTiles))
                {
                    forceRespectTiles = true;
                }
                else
                {
                    forceRespectTiles = false;
                }
            }

            // If the entity has a velocity, we'll move around by checking for collisions first.
            if (e.TryGetVelocity()?.Velocity is Vector2 currentVelocity)
            {
                Vector2 velocity = currentVelocity * Game.FixedDeltaTime;
                if (float.IsNaN(currentVelocity.X) || float.IsNaN(currentVelocity.Y))
                {
                    GameLogger.Warning("Non-Sane velocity detected, aborting");
                    e.SetVelocity(Vector2.Zero);
                    continue;
                }

                PositionComponent relativeStartPosition = e.GetPosition();
                Vector2 startPosition = relativeStartPosition.GetGlobal();

                // This is moving too slowly, checking for collisions is not necessary.
                if ((startPosition + velocity).Round() == startPosition.Round())
                {
                    ignoreCollisions = true;
                }

                if (collider is null)
                {
                    ignoreCollisions = true;
                }
                else if (!collider.Value.HasLayer(CollisionLayersBase.ACTOR))
                {
                    ignoreCollisions = true;
                }

                if (ignoreCollisions && !forceRespectTiles)
                {
                    e.SetGlobalPosition(startPosition + velocity);
                }
                else
                {
                    _entityList.Clear();
                    _hitCounter.Clear();
                    _cachedCandidates.Clear();

                    IntRectangle boundingBox = collider!.Value.GetBoundingBox(startPosition + velocity, e.FetchScale());

                    if (!ignoreCollisions)
                    {
                        qt.GetCollisionEntitiesAt(boundingBox, _entityList);
                        PhysicsServices.FilterPositionAndColliderEntities(_entityList, _ignore, mask, result: _cachedCandidates);
                    }

                    int exhaustCounter = ExhaustLimit;

                    bool collided = false;
                    int hitId = -1;
                    int hitLayer = -1;

                    Vector2 pushout;
                    bool Step(Vector2 moveToPosition)
                    {
                        // If the velocity is low, we can just check the full velocity.
                        while (PhysicsServices.GetFirstMtvAt(map, collider.Value, startPosition, moveToPosition, _cachedCandidates, mask, out int id, out int layer, out pushout)
                            && exhaustCounter-- > 0)
                        {
                            moveToPosition = moveToPosition - pushout;
                            _hitCounter.Add(id);
                            hitLayer = layer;
                            hitId = id;
                            collided = true;
                        }

                        var endPosition = (moveToPosition - pushout);
                        if (exhaustCounter <= 0 && _hitCounter.Count >= 2)
                        {
                            // Is stuck between 2 (or more!) objects, don't move at all.
                        }
                        else
                        {
                            // No collision found, we can move to the desired position.

                            // Round position if we're not moving in that axis so we avoid jittering in the long run.
                            if (velocity.X == 0)
                            {
                                endPosition.X = MathF.Round(endPosition.X);
                            }
                            if (velocity.Y == 0)
                            {
                                endPosition.Y = MathF.Round(endPosition.Y);
                            }

                            e.SetGlobalPosition(endPosition);
                        }

                        // Some collision was found!
                        if (collided)
                        {
                            e.SendCollidedWithMessage(hitId, hitLayer);

                            if (e.HasStopOnCollision())
                            {
                                currentVelocity = Vector2.Zero;
                            }
                            else
                            {
                                // Slide the speed accordingly
                                Vector2 translationVector = startPosition + velocity - moveToPosition;
                                Vector2 edgePerpendicularToMTV = new Vector2(-translationVector.Y, translationVector.X);
                                Vector2 normalizedEdge = edgePerpendicularToMTV.Normalized();
                                float dotProduct = Vector2.Dot(currentVelocity, normalizedEdge);
                                currentVelocity = normalizedEdge * dotProduct;
                            }
                        }

                        return collided;
                    }

                    float velocityLength = velocity.Length();
                    if (velocityLength > minStepSize)
                    {
                        // If the velocity is too high, we need to split it up into smaller steps.
                        bool fail = false;
                        for (int i = 0; i < velocityLength; i += minStepSize)
                        {
                            Vector2 step = velocity.Normalized() * minStepSize;
                            Vector2 targetPosition = startPosition + step;

                            if (Step(targetPosition))
                            {
                                fail = true;
                                break;
                            }
                        }

                        if (!fail)
                        {
                            Vector2 targetPosition = startPosition + velocity;
                            Step(targetPosition);
                        }
                    }
                    else
                    {
                        Vector2 targetPosition = startPosition + velocity;

                        Step(targetPosition);
                    }

                }

                // Makes sure that the velocity snaps to zero if it's too small.
                if (currentVelocity.Manhattan() > MinVelocityMagnitude)
                {
                    e.SetVelocity(currentVelocity);
                }
                else
                {
                    e.RemoveVelocity();
                    if (e.GetGlobalPositionIfValid() is Vector2 globalPosition)
                    {
                        e.SetGlobalPosition(globalPosition.Round());
                    }
                }
            }
        }
    }
}