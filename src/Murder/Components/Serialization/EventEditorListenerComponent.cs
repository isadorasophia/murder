using Bang.Components;
using Bang.Interactions;
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

        public readonly ImmutableArray<IInteractiveComponent>? Interactions { get; init; } = null;

        public SpriteEventInfo(string id) => Id = id;

        public SpriteEventInfo(string id, SoundEventId? sound, SoundLayer? persisted, ImmutableArray<IInteractiveComponent>? interactions)
        {
            Id = id;
            Sound = sound;
            Persisted = persisted;
            Interactions = interactions;
        }

        public SpriteEventInfo WithPersist(SoundLayer persisted) =>
            new(Id, Sound, persisted, Interactions);
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