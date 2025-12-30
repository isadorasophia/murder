using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Diagnostics;
using Murder.Utilities;

namespace Murder.Interactions;

public readonly struct PropagateInteraction : IInteraction
{
    public readonly TargetEntity Target = TargetEntity.Parent;

    public PropagateInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted is null)
        {
            return;
        }

        Entity? target;
        switch (Target)
        {
            case TargetEntity.Self:
                GameLogger.Error($"Unable to send interaction to self: {interacted.EntityId}");
                return;

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

            case TargetEntity.Child:
                foreach (int childId in interacted.Children)
                {
                    if (world.TryGetEntity(childId) is Entity child)
                    {
                        child.SendInteractMessage(interactor);
                    }
                }

                return;

            default:
                GameLogger.Error($"Unsupported {Target} for {nameof(PropagateInteraction)}.");
                return;
        }

        target?.SendInteractMessage(interactor);
    }
}
