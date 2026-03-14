using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Components;
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

        if (msg.Movement == CollisionDirection.Exit)
        {
            foreach (var interaction in interactOnCollision.CustomExitMessages)
            {
                interaction.Interact(world, interactorEntity, interactiveEntity);
            }

            if (!interactOnCollision.Flags.HasFlag(InteractOnCollisionFlags.InteractOnEnterAndExit))
            {
                return;
            }
        }
        else if (msg.Movement == CollisionDirection.Enter)
        {
            foreach (IInteractiveComponent interaction in interactOnCollision.CustomEnterMessages)
            {
                interaction.Interact(world, interactorEntity, interactiveEntity);
            }
        }

        // After all these checks, I thinks it's ok to send that message!            
        // Trigger right away!

        interactiveEntity.SendInteractMessage(interactorEntity);

        if (oncePerMap || interactOnCollision.Flags.HasFlag(InteractOnCollisionFlags.Once))
        {
            if (interactOnCollision.CustomExitMessages.Length > 0 && msg.Movement != CollisionDirection.Exit)
            {
                // wait until we get out before we deactivate ourselves.
                return;
            }

            if (oncePerMap)
            {
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
