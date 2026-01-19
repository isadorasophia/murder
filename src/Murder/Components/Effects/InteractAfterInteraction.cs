using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct InteractAfterInteraction : IInteraction
{
    public readonly float WaitFor = 0;

    public readonly ImmutableArray<IInteractiveComponent> Actions = [];

    public InteractAfterInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        ImmutableArray<IInteractiveComponent> actions = Actions;
        world.FireAfter(WaitFor, () =>
        {
            foreach (IInteractiveComponent a in actions)
            {
                a.Interact(world, interactor, interacted);
            }
        });
    }
}
