using Bang.Components;
using Murder.Attributes;
using System.Numerics;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct MoveToTargetComponent : IComponent
{
    public readonly int Target;
    public readonly float MinDistance = 4;
    public readonly float SlowDownDistance = 12;
    public readonly Vector2 Offset;
    
    public MoveToTargetComponent(int target, float minDistance, float slowDownDistance) : this(target, minDistance, slowDownDistance, Vector2.Zero)
    { }

    public MoveToTargetComponent(int target, float minDistance, float slowDownDistance, Vector2 offset)
    {
        Target = target;
        MinDistance = minDistance;
        SlowDownDistance = slowDownDistance;
        Offset = offset;
    }
}
