using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;

namespace Murder.Interactions;

public readonly struct ToggleActivateTargetInteraction : IInteraction
{
    public readonly string Target = string.Empty;

    public readonly bool Activate = true;

    public ToggleActivateTargetInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        // Sort of a workaround at this point.
        // For the dialogue entities, the original speaker entity is set in the IdTarget, which is the one we
        // are interested at. If this exists, use it as a target for finding "Target". Otherwise, no-op.
        Entity? targetToLookFor = interacted;
        if (interacted?.TryGetIdTarget()?.Target is int entityId && world.TryGetEntity(entityId) is Entity idTarget)
        {
            targetToLookFor = idTarget;
        }

        if (targetToLookFor is null)
        {
            return;
        }

        if (targetToLookFor.FetchTarget(world, Target) is Entity target)
        {
            if (Activate)
            { 
                target.Activate();
            }
            else 
            {
                target.Deactivate();
            }
        }
    }

}
