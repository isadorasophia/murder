using Bang;
using Bang.Components;
using Murder.Helpers;
using Murder.Utilities;
using System.Diagnostics;

namespace Murder.Components;

[KeepOnReplace]
public readonly struct FacingComponent : IComponent
{
    /// <summary>
    /// The angle the agent is facing, in radians
    /// </summary>
    public readonly float Angle;

    /// <summary>
    /// The <see cref="Direction"/> that this entity is facing
    /// </summary>
    public readonly Direction Direction => _direction ?? DirectionHelper.FromAngle(Angle);

    [Serialize]
    private readonly Direction? _direction;

    /// <summary>
    /// Creates a FacingComponent using a Direction as a base.
    /// </summary>
    /// <param name="direction"></param>
    public FacingComponent(Direction direction)
    {
        _direction = direction;
        Angle = Calculator.NormalizeAngle(direction.ToAngle());
    }

    public FacingComponent(float angle)
    {
        Angle = (angle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);
        _direction = DirectionHelper.FromAngle(angle);
    }
}