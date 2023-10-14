using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Graphics
{
    public readonly struct AnimationEventMessage : IMessage
    {
        public readonly string Event;
        public bool Is(ReadOnlySpan<char> eventId)
        {
            return eventId.Equals(Event.AsSpan(), StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Is(string eventId)
        {
            return eventId.Equals(Event, StringComparison.InvariantCultureIgnoreCase);
        }

        public AnimationEventMessage(string eventId)
        {
            Event = eventId;
        }
    }
}