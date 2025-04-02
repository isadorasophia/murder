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
        public readonly SoundLayer? Persisted = null;

        public SpriteEventInfo(string id) => Id = id;

        public SpriteEventInfo(string id, SoundEventId? sound, SoundLayer? persisted)
        {
            Id = id;
            Sound = sound;
            Persisted = persisted;
        }

        public SpriteEventInfo WithPersist(SoundLayer persisted) =>
            new(Id, Sound, persisted);
    }

    [SoundPlayer]
    public readonly struct EventListenerEditorComponent : IComponent
    {
        [Tooltip("Events triggered by an animation")]
        public readonly ImmutableArray<SpriteEventInfo> Events { get; init; } = [];

        public EventListenerEditorComponent() { }

        public EventListenerEditorComponent(ImmutableArray<SpriteEventInfo> events) => Events = events;
    }
}