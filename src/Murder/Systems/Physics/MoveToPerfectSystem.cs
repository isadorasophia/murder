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

namespace Murder.Systems
{
    /// <summary>
    /// Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.
    /// </summary>
    [Filter(typeof(ITransformComponent), typeof(MoveToPerfectComponent))]
    public class MoveToPerfectSystem : IFixedUpdateSystem, IStartupSystem
    {
        ImmutableArray<PhysicsServices.PhysicEntityCachedInfo>? _actorsCache = null;

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
                    _actorsCache = PhysicsServices.FilterPositionAndColliderEntities(context.World, CollisionLayersBase.ACTOR);
                    break;
                }
            }
            if (!anyActorAvoidant)
            {
                _actorsCache = null;
            }

            foreach (Entity e in context.Entities)
            {
                MoveToPerfectComponent moveToPerfect = e.GetMoveToPerfect();
                if (moveToPerfect.StartPosition is not Vector2 startPosition)
                {
                    startPosition = e.GetGlobalTransform().Vector2;
                    e.SetMoveToPerfect(moveToPerfect.WithStartPosition(startPosition));
                }

                double delta = Calculator.Clamp01((Game.Now - moveToPerfect.StartTime) / moveToPerfect.Duration);
                double easedDelta = Ease.Evaluate(delta, moveToPerfect.EaseKind);

                Vector2 current = Vector2Helper.LerpSnap(startPosition, moveToPerfect.Target, easedDelta);
                e.SetGlobalTransform(e.GetMurderTransform().With(current.Point()));

                if (anyActorAvoidant && moveToPerfect.AvoidActors && _actorsCache != null && e.TryGetCollider() is ColliderComponent collider)
                {
                    Vector2 position = e.GetGlobalTransform().Vector2;

                    // Avoid actors
                    if (PhysicsServices.GetFirstMtv(e.EntityId, collider, position, _actorsCache, out int hit) is Vector2 mtv)
                    {
                        if (context.World.TryGetEntity(hit) is Entity actor)
                        {
                            actor.SetGlobalTransform(actor.GetGlobalTransform().With(actor.GetGlobalTransform().Vector2 + mtv));
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