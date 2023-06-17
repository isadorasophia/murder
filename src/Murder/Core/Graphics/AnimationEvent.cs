using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Graphics
{
    public readonly struct AnimationEvent : IMessage
    {
        public readonly string Event;

        public AnimationEvent(string @event)
        {
            Event = @event;
        }
    }
}
