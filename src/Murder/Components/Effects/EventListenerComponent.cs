using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct EventListenerComponent : IComponent
    {
        [Tooltip("Events triggered by an animation")]
        public readonly ImmutableDictionary<string, SpriteEventInfo> Events = ImmutableDictionary<string, SpriteEventInfo>.Empty;

        public EventListenerComponent() { }

        public EventListenerComponent(ImmutableDictionary<string, SpriteEventInfo> events) =>
            Events = events;
    }
}
