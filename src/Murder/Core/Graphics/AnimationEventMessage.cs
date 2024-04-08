using Bang.Components;
using Murder.Attributes;
using Newtonsoft.Json;

namespace Murder.Core.Graphics
{
    public readonly struct AnimationEventMessage : IMessage
    {
        public readonly string Event;
        
        /// <summary>
        /// This AnimationEvent is being broadcasted from another entity.
        /// Right now this is only for debug purposes.
        /// </summary>
        [JsonProperty, Bang.Serialize]
        [HideInEditor]
        public readonly bool BroadcastedEvent { get; init; } = false;

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