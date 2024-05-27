using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Messages;

public readonly struct ThetherSnapMessage : IMessage
{
    public readonly int AttachedEntityId;

    public ThetherSnapMessage(int attachedEntityId)
    {
        AttachedEntityId = attachedEntityId;
    }
}
