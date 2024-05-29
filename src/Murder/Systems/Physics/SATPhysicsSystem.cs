﻿using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Physics;


[Filter(typeof(ITransformComponent), typeof(VelocityComponent))]
public class SATPhysicsSystem : IFixedUpdateSystem
{
    private const int ExhaustLimit = 10;
    private const float MinVelocityMagnitude = 0.0001f;

    private readonly List<NodeInfo<Entity>> _entityList = new();
    private readonly HashSet<int> _ignore = new();
    private readonly HashSet<int> _hitCounter = new();

    public void FixedUpdate(Context context)
    {
        Map map = context.World.GetUniqueMap().Map;
        Quadtree qt = Quadtree.GetOrCreateUnique(context.World);

        _entityList.Clear();

        foreach (Entity e in context.Entities)
        {
            bool ignoreCollisions = false;

            var collider = e.TryGetCollider();
            _ignore.Clear();
            _ignore.Add(e.EntityId);

            if (e.Parent is not null)
                _ignore.Add(e.Parent.Value);

            foreach (var child in e.Children)
            {
                _ignore.Add(child);
            }

            int mask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;
            if (e.TryGetCustomCollisionMask() is CustomCollisionMask agent)
                mask = agent.CollisionMask;

            if (e.HasFreeMovement())
            {
                ignoreCollisions = true;
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
                IMurderTransformComponent relativeStartPosition = e.GetMurderTransform();
                Vector2 startPosition = relativeStartPosition.GetGlobal().ToVector2();

                
                // This is moving too slowly, checking for collisions is not necessary.
                if ((startPosition + velocity).Round() == startPosition.Round())
                {
                    ignoreCollisions = true;
                }

                if (collider is not null && collider.Value.Layer == CollisionLayersBase.TRIGGER)
                {
                    ignoreCollisions = true;
                }

                if (collider is null)
                {
                    ignoreCollisions = true;
                }

                if (ignoreCollisions)
                {
                    e.SetGlobalPosition(startPosition + velocity);
                }
                else
                {
                    _entityList.Clear();
                    _hitCounter.Clear();

                    qt.GetCollisionEntitiesAt(collider!.Value.GetBoundingBox(startPosition + velocity), _entityList);
                    var collisionEntities = PhysicsServices.FilterPositionAndColliderEntities(_entityList, mask);

                    int exhaustCounter = ExhaustLimit;

                    bool collided = false;
                    int hitId = -1;
                    int hitLayer = -1;
                    Vector2 moveToPosition = startPosition + velocity;
                    Vector2 pushout;

                    while (PhysicsServices.GetFirstMtvAt(map, _ignore, collider.Value, moveToPosition, collisionEntities, mask, out int id, out int layer, out pushout)
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
                        e.SetGlobalPosition(endPosition);   
                    }

                    // Some collision was found!
                    if (collided)
                    {
                        e.SendMessage(new CollidedWithMessage(hitId, hitLayer));

                        // Slide the speed accordingly
                        Vector2 translationVector = startPosition + velocity - moveToPosition;
                        Vector2 edgePerpendicularToMTV = new Vector2(-translationVector.Y, translationVector.X);
                        Vector2 normalizedEdge = edgePerpendicularToMTV.Normalized();
                        float dotProduct = Vector2.Dot(currentVelocity, normalizedEdge);
                        currentVelocity = normalizedEdge * dotProduct;
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
                }
            }
        }
    }
}