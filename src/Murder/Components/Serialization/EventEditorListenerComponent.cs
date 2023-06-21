using Bang.Components;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct SpriteEventInfo
    {
        [Tooltip("Id listed in sprite")]
        public readonly string Id;

        [Default("Add sound")]
        public readonly SoundEventId? Sound;

        public SpriteEventInfo(string id) => Id = id;
    }

    [SoundPlayer]
    public readonly struct EventListenerEditorComponent : IComponent
    {
        [Tooltip("Events triggered by an animation")]
        public readonly ImmutableArray<SpriteEventInfo> Events = ImmutableArray<SpriteEventInfo>.Empty;

        public EventListenerEditorComponent() { }
    }
}
