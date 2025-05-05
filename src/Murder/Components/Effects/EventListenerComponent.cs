using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Components;

[RuntimeOnly]
[PersistOnSave]
public readonly struct EventListenerComponent : IComponent
{
    [Tooltip("Events triggered by an animation")]
    public readonly ImmutableDictionary<string, SpriteEventInfo> Events = ImmutableDictionary<string, SpriteEventInfo>.Empty;

    public EventListenerComponent() { }

    [JsonConstructor]
    public EventListenerComponent(ImmutableDictionary<string, SpriteEventInfo> events) =>
        Events = events;

    public EventListenerComponent Merge(ImmutableDictionary<string, SpriteEventInfo> events)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, SpriteEventInfo>();

        builder.AddRange(Events);

        foreach ((string id, SpriteEventInfo info) in events)
        {
            builder[id] = info;
        }

        return new(builder.ToImmutable());
    }
}

[RuntimeOnly]
public readonly struct PlayingPersistedEventComponent : IComponent { }