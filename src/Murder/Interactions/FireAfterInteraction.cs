using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Interactions;

public readonly struct FireAfterInteraction : IInteraction
{
    public readonly ImmutableArray<IInteractiveComponent> Actions = [];
    public readonly float Seconds = 0;

    public FireAfterInteraction() { }

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (Seconds == 0)
        {
            DialogueServices.Fire(world, interactor, interacted, Actions);
        }
        else
        {
            ImmutableArray<IInteractiveComponent> actions = Actions;
            world.FireAfter(Seconds, () => DialogueServices.Fire(world, interactor, interacted, actions));
        }
    }
}
