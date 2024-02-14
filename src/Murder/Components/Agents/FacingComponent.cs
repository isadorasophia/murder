using Bang.Components;
using Murder.Helpers;
using Murder.Utilities;
using System.Text.Json.Serialization;

namespace Murder.Components;

public readonly struct FacingComponent : IComponent
{
    /// <summary>
    /// The angle the agent is facing, in radians
    /// </summary>
    public readonly float Angle;

    /// <summary>
    /// The <see cref="Direction"/> that this entity is facing
    /// </summary>
    [JsonIgnore]
    public readonly Direction Direction { 
        get => _direction; 
        init {
            Angle = Calculator.NormalizeAngle(value.ToAngle());
            _direction = value;
        }
    }

    /// <summary>
    /// Cache
    /// </summary>
    public readonly Direction _direction;

    /// <summary>
    /// Creates a FacingComponent using a Direction as a base.
    /// </summary>
    /// <param name="direction"></param>
    public FacingComponent(Direction direction)
    {
        Direction = direction;
        Angle = Calculator.NormalizeAngle(direction.ToAngle());
    }

    public FacingComponent(float angle)
    {
        Angle = (angle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);
        Direction = DirectionHelper.FromAngle(Angle);
    }
}