using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Messages.Physics
{
    public readonly struct OnTriggerExitMessage : IMessage
    {
        public readonly int EntityId;

        public OnTriggerExitMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}
