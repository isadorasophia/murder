using Bang.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct MovementModAreaComponent : IComponent
{
    [Tooltip("Only affect agents that are grounded")]
    public readonly bool GroundedOnly = true;
    public readonly float SpeedMultiplier = 0f;

    [Tooltip("Slide direction and strength. Consider Top and Right for Horizontal and Vertical.")]
    public readonly float Slide;

    public readonly Orientation Orientation;
    
    public readonly Tags AffectOnly = new Tags();

    public MovementModAreaComponent()
    {
    }
}

public struct AreaInfo
{
    public readonly float SpeedMultiplier = 0f;
    public readonly Orientation Orientation;
    public readonly float Slide;

    public AreaInfo(MovementModAreaComponent area)
    {
        SpeedMultiplier = area.SpeedMultiplier;
        Orientation = area.Orientation;
        Slide = area.Slide;
    }

    public AreaInfo(float multiplier)
    {
        SpeedMultiplier = multiplier;

        Orientation = Orientation.Both;
        Slide = 0;
    }
}

[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct InsideMovementModAreaComponent : IComponent
{
    public readonly ImmutableArray<AreaInfo> Areas;

    public InsideMovementModAreaComponent(MovementModAreaComponent area) : this(new AreaInfo(area)) { }

    public InsideMovementModAreaComponent(AreaInfo info) : this([info]) { }

    public InsideMovementModAreaComponent(ImmutableArray<AreaInfo> areas)
    {
        Areas = areas;
    }

    public InsideMovementModAreaComponent AddArea(AreaInfo info)
    {
        return new InsideMovementModAreaComponent(Areas.Add(info));
    }

    public InsideMovementModAreaComponent? RemoveArea(AreaInfo info)
    {
        var index = Areas.IndexOf(info);
        if (index < 0 || index >= Areas.Length)
        {
            return null;
        }

        return new InsideMovementModAreaComponent(Areas.RemoveAt(index));
    }

    public InsideMovementModAreaComponent AddArea(MovementModAreaComponent area) => AddArea(new AreaInfo(area));

    public InsideMovementModAreaComponent? RemoveArea(MovementModAreaComponent area) => RemoveArea(new AreaInfo(area));

    public AreaInfo? GetLatest()
    {
        if (Areas.IsEmpty)
        {
            return null;
        }

        return Areas[Areas.Length - 1];
    }
}