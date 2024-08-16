using Bang;
using Bang.Components;
using Bang.Entities;
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
        if (!IsInteractAllowed(interactorEntity, interactOnCollision))
        {
            return;
        }

        if (msg.Movement == CollisionDirection.Exit)
        {
            foreach (var interaction in interactOnCollision.CustomExitMessages)
            {
                interaction.Interact(world, interactorEntity, interactiveEntity);
            }

            if (!interactOnCollision.SendMessageOnExit)
                return;
        }
        else if (msg.Movement == CollisionDirection.Enter)
        {
            foreach (var interaction in interactOnCollision.CustomEnterMessages)
            {
                interaction.Interact(world, interactorEntity, interactiveEntity);
            }
        }

        // After all these checks, I thinks it's ok to send that message!            
        // Trigger right away!

        interactiveEntity.SendInteractMessage(interactorEntity);

        if (interactOnCollision.OnlyOnce)
        {
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

    protected virtual bool IsInteractAllowed(Entity interactor, InteractOnCollisionComponent component)
    {
        return true;
    }
}
