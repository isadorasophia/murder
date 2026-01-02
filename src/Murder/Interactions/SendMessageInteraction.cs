using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Utilities;

namespace Murder.Interactions;

public readonly struct SendMessageInteraction : IInteraction
{
    public readonly IMessage? Message = null;

    public readonly TargetEntity Target = TargetEntity.Self;

    public SendMessageInteraction() { }

    public SendMessageInteraction(IMessage? message, TargetEntity target) =>
        (Message, Target) = (message, target);

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
                if (interacted.TryGetIdTarget()?.Target is int entityId)
                {
                    target = world.TryGetEntity(entityId);
                    break;
                }

                if (interacted.TryGetIdTargetCollection() is IdTargetCollectionComponent targets)
                {
                    foreach ((_, int entityTargetId) in targets.Targets)
                    {
                        if (world.TryGetEntity(entityTargetId) is Entity child)
                        {
                            child.SendMessage(Message);
                        }
                    }
                }

                return;

            case TargetEntity.Child:
                foreach (int childId in interacted.Children)
                {
                    if (world.TryGetEntity(childId) is Entity child)
                    {
                        child.SendMessage(Message);
                    }
                }

                return;

            default:
                GameLogger.Error($"Unsupported {Target} for {nameof(SendMessageInteraction)}.");
                return;
        }

        target?.SendMessage(Message);
    }
}
