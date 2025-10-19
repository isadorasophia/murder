using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
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

    [Tooltip("Plays event when following conditions are met. Checks every map reload or time slot change.")]
    public readonly ImmutableArray<CriterionNode>? OnlyWhen = null;

    public SoundEventIdInfo() { }
}

public enum ListenerKind
{
    Camera = 0, // Default
    Player = 1
}

/// <summary>
/// Component that will listen to interaction Areas for starting and stopping
/// ambience sounds.
/// </summary>
[CustomName($"\uf186 {nameof(AmbienceComponent)}")]
[Sound]
public readonly struct AmbienceComponent : IComponent
{
    public readonly ListenerKind Listener = ListenerKind.Camera;
    public readonly ImmutableArray<SoundEventIdInfo> Events = [];

    public AmbienceComponent() { }
}
