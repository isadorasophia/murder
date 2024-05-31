using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Interactions;

public readonly struct SendMessageInteraction : IInteraction
{
    public readonly IMessage Message;
    public void Interact(World world, Entity interactor, Entity? interacted)
    {
        interacted?.SendMessage(Message);
    }
}
