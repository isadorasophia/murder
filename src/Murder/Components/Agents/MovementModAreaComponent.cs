using Bang.Components;
using Murder.Core;

namespace Murder.Components;

public readonly struct MovementModAreaComponent : IComponent
{
    public readonly float SpeedMultiplier = 0f;
    public readonly Orientation Orientation;

    public MovementModAreaComponent()
    {
    }
}


public readonly struct InsideMovementModAreaComponent : IComponent
{
    public readonly float SpeedMultiplier = 0f;
    public readonly Orientation Orientation;

    public InsideMovementModAreaComponent(MovementModAreaComponent area)
    {
        SpeedMultiplier = area.SpeedMultiplier;
        Orientation = area.Orientation;
    }
}
