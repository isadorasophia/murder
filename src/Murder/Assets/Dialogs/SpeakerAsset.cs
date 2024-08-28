using Murder.Assets.Sounds;
using Murder.Attributes;
using Murder.Core;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Assets;

[Flags]
public enum PortraitProperties
{
    None = 0,
    Loop = 1
}

public readonly struct PortraitInfo
{
    public readonly Portrait Portrait { get; init; } = new();

    public readonly PortraitProperties Properties { get; init; } = PortraitProperties.Loop; 

    public PortraitInfo() { }
}

public class SpeakerAsset : GameAsset
{
    public override char Icon => '\uf2c1';
    public override string EditorFolder => "#\uf518Story\\#\uf2c1Speakers";

    [Tooltip("Name used on scripts and to reference this speaker")]
    public readonly string SpeakerName = string.Empty;

    [Tooltip("Portrait that will be shown by default, if none is specified.")]
    public readonly string? DefaultPortrait = "Idle";

    [Tooltip("Speaker events")]
    public readonly AssetRef<SpeakerEventsAsset>? Events = null;

    public readonly Portrait? CustomBox;

    public readonly float fade = 0.45f;

    [Font]
    public readonly int? CustomFont;

    public readonly ImmutableDictionary<string, PortraitInfo> Portraits = ImmutableDictionary<string, PortraitInfo>.Empty;
}