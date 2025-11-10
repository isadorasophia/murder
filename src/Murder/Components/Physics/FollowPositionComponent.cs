using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components;

public enum FollowPositionFacingProperties
{
    None = 0,
    SameAsTarget = 1,
    LookAtTarget = 2
}

/// <summary>
/// Component that wants to follow another entity.
/// </summary>
[RuntimeOnly]
[DoNotPersistOnSave]
public readonly struct FollowPositionComponent : IComponent
{
    /// <summary>
    /// Entity that the position is being tracked.
    /// </summary>
    public readonly int EntityId;

    public readonly Vector2 InitialPosition;

    public readonly FollowPositionFacingProperties Facing = FollowPositionFacingProperties.None;

    public FollowPositionComponent(int id, Vector2 initialPosition) => (EntityId, InitialPosition) = (id, initialPosition);

    public FollowPositionComponent(int id, Vector2 initialPosition, FollowPositionFacingProperties facing) : this(id, initialPosition) => Facing = facing; 
}