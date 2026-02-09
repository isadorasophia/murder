using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using static Murder.Services.PhysicsServices;

namespace Murder.Systems
{
    /// <summary>
    /// Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.
    /// </summary>
    [Filter(typeof(PositionComponent), typeof(MoveToPerfectComponent))]
    public class MoveToPerfectSystem : IFixedUpdateSystem, IStartupSystem
    {
        private readonly List<PhysicEntityCachedInfo> _cachedCandidates = [];

        public void Start(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                MoveToPerfectComponent moveToPerfect = e.GetMoveToPerfect();

                // Do not persist MoveToPerfect. Snap to position immediately.
                e.SetGlobalPosition(moveToPerfect.Target);
                e.RemoveMoveToPerfect();
            }
        }

        public void FixedUpdate(Context context)
        {
            bool anyActorAvoidant = false;

            foreach (Entity e in context.Entities)
            {
                MoveToPerfectComponent moveToPerfect = e.GetMoveToPerfect();
                if (moveToPerfect.AvoidActors)
                {
                    anyActorAvoidant = true;
                    PhysicsServices.FilterPositionAndColliderEntities(context.World, CollisionLayersBase.ACTOR, result: _cachedCandidates);
                    break;
                }
            }

            if (!anyActorAvoidant)
            {
                _cachedCandidates.Clear();
            }

            foreach (Entity e in context.Entities)
            {
                MoveToPerfectComponent moveToPerfect = e.GetMoveToPerfect();
                if (moveToPerfect.StartPosition is not Vector2 startPosition)
                {
                    startPosition = e.GetGlobalPosition();
                    e.SetMoveToPerfect(moveToPerfect with { StartPosition = startPosition });
                }

                double delta = Calculator.Clamp01((Game.Now - moveToPerfect.StartTime) / moveToPerfect.Duration);
                double easedDelta = Ease.Evaluate(delta, moveToPerfect.EaseKind);

                Vector2 current = Vector2Helper.LerpSnap(startPosition, moveToPerfect.Target, easedDelta);
                e.SetGlobalPosition(current.Point());

                if (anyActorAvoidant && moveToPerfect.AvoidActors && _cachedCandidates.Count != 0 && e.TryGetCollider() is ColliderComponent collider)
                {
                    Vector2 position = e.GetGlobalPosition();

                    // Avoid actors
                    if (PhysicsServices.GetFirstMtv(e.EntityId, collider, position, _cachedCandidates, out int hit) is Vector2 mtv)
                    {
                        if (context.World.TryGetEntity(hit) is Entity actor)
                        {
                            actor.SetGlobalPosition(actor.GetGlobalPosition() + mtv);
                        }
                    }
                }

                if (delta >= 1)
                {
                    e.RemoveMoveToPerfect();
                    // Send message?
                }
            }
        }
    }
}