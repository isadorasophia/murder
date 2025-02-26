using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;

namespace Murder.Interactions;

public readonly struct EnableChildrenInteraction : IInteraction
{
    public readonly string? Target = null;

    public EnableChildrenInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted == null)
        {
            return;
        }

        if (Target is not null && interacted.FetchTarget(world, Target) is Entity target)
        {
            interacted = target;
        }
        
        Enable(world, interacted);
    }

    public static void Enable(World world, Entity target)
    {
        foreach (var c in target.Children)
        {
            if (world.TryGetEntity(c) is Entity child)
            {
                if (child.IsActive)
                {
                    continue;
                }

                if (child.HasSprite())
                {
                    child.SetAnimationStarted(Game.Now);
                }

                child.Activate();
            }
        }
    }
}