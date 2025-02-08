using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;
using Murder.Utilities.Attributes;

namespace Murder.Interactions;

public readonly struct DeactivateChildForTargetInteraction : IInteraction
{
    [Target]
    public readonly string? Target = null;

    public readonly string ChildName = string.Empty;
    public readonly bool ActivateInstead = false;

    public DeactivateChildForTargetInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted is null)
        {
            return;
        }

        int? targetId = Target is not null ? interacted.FindTarget(Target) : 
            interacted.TryGetIdTarget()?.Target;

        if (targetId is null)
        {
            return;
        }

        Entity? target = world.TryGetEntity(targetId.Value);
        if (target is null)
        {
            return;
        }

        Entity? child = target.TryFetchChild(ChildName);
        if (child is null)
        {
            return;
        }

        if (ActivateInstead)
        {
            child.Activate();
        }
        else
        {
            child.Deactivate();
        }
    }
}
