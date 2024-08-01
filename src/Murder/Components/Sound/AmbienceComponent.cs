using Bang.Components;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct SoundEventIdInfo
{
    public readonly SoundEventId Id = new();

    /// <summary>
    /// This is useful if this is actually a snapshot, for example.
    /// </summary>
    public readonly SoundLayer Layer = SoundLayer.Ambience;

    public SoundEventIdInfo() { }
}

/// <summary>
/// Component that will listen to interaction Areas for starting and stopping
/// ambience sounds.
/// </summary>
[CustomName("\uf186 Ambience component")]
public readonly struct AmbienceComponent : IComponent
{
    public readonly ImmutableArray<SoundEventIdInfo> Events = [];

    public AmbienceComponent() { }
}
