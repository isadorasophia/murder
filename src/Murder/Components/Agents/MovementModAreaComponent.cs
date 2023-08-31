using Bang.Components;
using Murder.Attributes;
using Murder.Core;
using Murder.Utilities.Attributes;
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

    public MovementModAreaComponent()
    {
    }
}

[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct InsideMovementModAreaComponent : IComponent
{
    public readonly float SpeedMultiplier = 0f;
    public readonly Orientation Orientation;
    public readonly float Slide;

    public InsideMovementModAreaComponent(MovementModAreaComponent area)
    {
        SpeedMultiplier = area.SpeedMultiplier;
        Orientation = area.Orientation;
        Slide = area.Slide;
    }
}
