using Bang.Components;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

/// <summary>
/// For now, this is only supported for aseprite components.
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

    public readonly bool FadeChildren = false;
    public FadeWhenInAreaComponent() { }
}