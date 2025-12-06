using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions;

public readonly struct DeactivateChildInteraction() : IInteraction
{
    [ChildId, Serialize, ShowInEditor]
    private readonly ImmutableArray<string> _child = [];

    [Serialize]
    private readonly bool _activateInstead = false;

    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        if (interacted is null)
        {
            return;
        }

        Entity target = EntityServices.FindRootEntity(interacted);
        foreach (string child in _child)
        {
            if (target.TryFetchChild(child) is Entity c)
            {
                if (_activateInstead)
                {
                    c.Activate();
                }
                else
                {
                    c.Deactivate();
                }
            }
        }
    }
}
