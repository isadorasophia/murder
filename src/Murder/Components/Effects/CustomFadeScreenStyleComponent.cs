using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;
using System.Text.Json.Serialization;

namespace Murder.Components;

public readonly struct CustomFadeScreenInfo
{
    public string? CustomFadeImage { get; init; } = null;

    // Default is 1
    public float? Duration { get; init; } = null;

    public Color? Color { get; init; } = null;

    /// <summary>
    /// Propagated from EventListener.
    /// </summary>
    [JsonIgnore]
    [HideInEditor]
    public SoundEventId? CustomSound { get; init; } = null;

    public CustomFadeScreenInfo() { }
}

[EventMessages("enter", "exit")]
[Unique]
public readonly struct CustomFadeScreenStyleComponent: IComponent
{
    public readonly CustomFadeScreenInfo Info = new();

    public CustomFadeScreenStyleComponent() { }
}
