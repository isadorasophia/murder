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

    public MovementModAreaComponent(bool groundedOnly, float speedMultiplier, float slide, Orientation orientation, Tags affectOnly)
    {
        GroundedOnly = groundedOnly;
        SpeedMultiplier = speedMultiplier;
        Slide = slide;
        Orientation = orientation;
        AffectOnly = affectOnly;
    }
}

public struct AreaInfo
{
    public readonly int AreaEntityId = -1;

    public readonly float SpeedMultiplier = 0f;
    public readonly Orientation Orientation;
    public readonly float Slide;

    public AreaInfo(int areaEntityId, MovementModAreaComponent area)
    {
        AreaEntityId = areaEntityId;

        SpeedMultiplier = area.SpeedMultiplier;
        Orientation = area.Orientation;
        Slide = area.Slide;
    }

    public AreaInfo(int areaEntityId, float multiplier)
    {
        AreaEntityId = areaEntityId;
        SpeedMultiplier = multiplier;

        Orientation = Orientation.Both;
        Slide = 0;
    }
}

[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct InsideMovementModAreaComponent : IComponent
{
    public readonly ImmutableArray<AreaInfo> Areas = [];

    public InsideMovementModAreaComponent(int areaEntityId, MovementModAreaComponent area) : this(new AreaInfo(areaEntityId, area)) { }

    public InsideMovementModAreaComponent(AreaInfo info) : this([info]) { }

    public InsideMovementModAreaComponent(ImmutableArray<AreaInfo> areas)
    {
        Areas = areas;
    }

    public InsideMovementModAreaComponent AddArea(AreaInfo info)
    {
        for (int i = 0; i < Areas.Length; ++i)
        {
            if (Areas[i].AreaEntityId == info.AreaEntityId)
            {
                // Component was already tracking this area! No-op.
                return this;
            }
        }

        return new InsideMovementModAreaComponent(Areas.Add(info));
    }

    public InsideMovementModAreaComponent? RemoveArea(AreaInfo info)
    {
        for (int i = 0; i < Areas.Length; ++i)
        {
            if (Areas[i].AreaEntityId == info.AreaEntityId)
            {
                ImmutableArray<AreaInfo> afterRemove = Areas.RemoveAt(i);
                if (afterRemove.Length != 0)
                {
                    return new(afterRemove);
                }
            }
        }

        return null;
    }

    public InsideMovementModAreaComponent AddArea(int areaEntityId, MovementModAreaComponent area) => AddArea(new AreaInfo(areaEntityId, area));

    public InsideMovementModAreaComponent? RemoveArea(int areaEntityId, MovementModAreaComponent area) => RemoveArea(new AreaInfo(areaEntityId, area));

    public AreaInfo? GetLatest()
    {
        if (Areas.IsEmpty)
        {
            return null;
        }

        return Areas[Areas.Length - 1];
    }
}