using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Diagnostics;
using Murder.Utilities;

namespace Murder.Interactions;

public readonly struct SendMessageInteraction : IInteraction
{
    public readonly IMessage? Message = null;

    public readonly TargetEntity Target = TargetEntity.Self;

    public SendMessageInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted is null || Message is null)
        {
            return; 
        }

        Entity? target;
        switch (Target)
        {
            case TargetEntity.Self:
                target = interacted;
                break;

            case TargetEntity.Parent:
                target = interacted.TryFetchParent();
                break;

            case TargetEntity.Interactor:
                target = interactor;
                break;

            case TargetEntity.Target:
                int entityId = interacted.TryGetIdTarget()?.Target ?? -1;
                target = world.TryGetEntity(entityId);
                break;

            default:
                GameLogger.Error("Unsupported TargetEntity for SendMessageInteraction.");
                target = null;
                break;
        }

        target?.SendMessage(Message);
    }
}
