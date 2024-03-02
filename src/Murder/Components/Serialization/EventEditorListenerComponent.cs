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

        [Default("Whether this should be persisted (such as ambience sounds)")]
        public readonly bool Persist = false;

        public SpriteEventInfo(string id) => Id = id;

        public SpriteEventInfo(string id, SoundEventId? sound, bool persist)
        {
            Id = id;
            Sound = sound;
            Persist = persist;
        }

        public SpriteEventInfo(string id, SoundEventId? sound)
        {
            Id = id;
            Sound = sound;
        }

        public SpriteEventInfo WithPersist(bool persist) =>
            new(Id, Sound, persist);
    }

    [SoundPlayer]
    public readonly struct EventListenerEditorComponent : IComponent
    {
        [Tooltip("Events triggered by an animation")]
        public readonly ImmutableArray<SpriteEventInfo> Events = ImmutableArray<SpriteEventInfo>.Empty;

        public EventListenerEditorComponent() { }
    }
}