using Bang;
using Bang.Entities;
using Bang.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions;

public readonly struct RemoveFromParentInteraction : IInteraction
{
    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        interacted?.Unparent();
    }
}
