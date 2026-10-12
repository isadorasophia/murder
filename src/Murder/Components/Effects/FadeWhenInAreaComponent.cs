using Bang.Components;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

/// <summary>
/// Whether alpha should be applied when in area.
/// </summary>
[Requires(typeof(ColliderComponent))]
public readonly struct FadeWhenInAreaComponent : IComponent
{
    public enum FadeWhenInAreaStyle
    {
        HideWhenInArea,
        ShowWhenInArea,
        HideForeverAfterInArea
    }

    public readonly float Duration = 0;
    public readonly FadeWhenInAreaStyle Style = FadeWhenInAreaStyle.HideWhenInArea;

    [Target]
    public readonly ImmutableArray<string> AppliesTo = ImmutableArray<string>.Empty;

    /// <summary>
    /// Applicable when broadcasting that visibility was toggled (with <see cref="HiddenStatusComponent"/>).
    /// </summary>
    [Target]
    public readonly TargetCollectionInfo? BroadcastTo = null;

    public readonly bool FadeChildren = false;

    public FadeWhenInAreaComponent() { }
}

public readonly struct TargetCollectionInfo
{
    [Target]
    public readonly ImmutableArray<string> Targets = [];

    /// <summary>
    /// Name of children that will be propagated from each target.
    /// </summary>
    public readonly string? PropagateToChildren = null;

    public TargetCollectionInfo() { }
}