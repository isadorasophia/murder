using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Messages.Physics;
using Murder.Utilities;

namespace Murder.Systems;

/// <summary>
/// Interactors tag hightlights and interacts with InteractorComponents
/// </summary>
[Filter(typeof(InteractOnCollisionComponent))]
[Messager(typeof(OnCollisionMessage))]
public class InteractOnCollisionSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        OnCollisionMessage msg = (OnCollisionMessage)message;

        if (world.TryGetEntity(msg.EntityId) is not Entity interactorEntity)
        {
            return;
        }

        if (interactorEntity.IsDestroyed ||
            (!interactorEntity.HasAgent() && (!interactorEntity.TryFetchParent()?.HasAgent() ?? true)) ||
            interactorEntity.HasIgnoreUntil())
        {
            return;
        }

        Entity interactiveEntity = entity;
        if (interactiveEntity.IsDestroyed || interactorEntity.HasIgnoreUntil())
        {
            return;
        }

        InteractOnCollisionComponent interactOnCollision = interactiveEntity.GetInteractOnCollision();
        bool oncePerMap = interactOnCollision.Flags.HasFlag(InteractOnCollisionFlags.OnceEveryLoad);

        if (oncePerMap && interactiveEntity.HasInteracted())
        {
            return;
        }

        if (!IsInteractAllowed(world, interactorEntity, interactiveEntity, interactOnCollision, msg))
        {
            return;
        }

        if (msg.Movement == CollisionDirection.Exit && interactOnCollision.CustomExitMessages.Length > 0)
        {
            bool skipSendingExitMessage = interactOnCollision.Flags.HasFlag(InteractOnCollisionFlags.SkipExitIfInteractorInside) && 
                IsInteractorInside(world, entity, interactorEntity.EntityId);

            if (!skipSendingExitMessage)
            {
                foreach (IInteractiveComponent interaction in interactOnCollision.CustomExitMessages)
                {
                    interaction.Interact(world, interactorEntity, interactiveEntity);
                }
            }
        }
        else if (msg.Movement == CollisionDirection.Enter)
        {
            foreach (IInteractiveComponent interaction in interactOnCollision.CustomEnterMessages)
            {
                interaction.Interact(world, interactorEntity, interactiveEntity);
            }

            // all right, we just entered, send that interact message.
            interactiveEntity.SendInteractMessage(interactorEntity);
        }

        if (oncePerMap || interactOnCollision.Flags.HasFlag(InteractOnCollisionFlags.Once))
        {
            if (interactOnCollision.CustomExitMessages.Length > 0 && msg.Movement != CollisionDirection.Exit)
            {
                // we still have things to do!!
                // wait until we get out before we deactivate ourselves.
                return;
            }

            if (oncePerMap)
            {
                // make sure we don't interact again.
                interactiveEntity.SetInteracted();
                return;
            }

            if (interactiveEntity.HasDeactivateAfterInteracted())
            {
                interactiveEntity.Deactivate();
            }
            else
            {
                interactiveEntity.RemoveInteractOnCollision();
            }
        }
    }

    private bool IsInteractorInside(World world, Entity e, int except)
    {
        if (e.TryGetCollisionCache() is not CollisionCacheComponent collisionCache)
        {
            return false;
        }

        foreach (int otherId in collisionCache.CollidingWith)
        {
            if (otherId == except)
            {
                continue;
            }

            Entity? other = world.TryGetEntity(otherId);
            if (other is null)
            {
                continue;
            }

            if (other.HasAgent())
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool IsInteractAllowed(
        World world,
        Entity interactor, 
        Entity interacted, 
        InteractOnCollisionComponent component,
        OnCollisionMessage message)
    {
        return true;
    }
}
