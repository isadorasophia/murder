using Bang.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

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

[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct InsideMovementModAreaComponent : IComponent
{
    public readonly ImmutableArray<AreaInfo> areas;
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
    }
    public InsideMovementModAreaComponent(MovementModAreaComponent area)
    {
        areas = ImmutableArray.Create(new AreaInfo(area));
    }

    public InsideMovementModAreaComponent(ImmutableArray<AreaInfo> areas)
    {
        this.areas = areas;
    }

    public InsideMovementModAreaComponent AddArea(MovementModAreaComponent area)
    {
        return new InsideMovementModAreaComponent(areas.Add(new AreaInfo(area)));
    }

    internal InsideMovementModAreaComponent? RemoveArea(MovementModAreaComponent area)
    {
        var index = areas.IndexOf(new AreaInfo(area));
        if (index < 0)
            return null;

        return new InsideMovementModAreaComponent(areas.RemoveAt(index));
    }

    internal AreaInfo? GetLatest()
    {
        if (areas.IsEmpty)
            return null;

        return areas[areas.Length - 1];
    }
}