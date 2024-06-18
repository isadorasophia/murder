using Bang.Components;
using System.Numerics;
namespace Murder.Messages;

public readonly struct RecalculatePathfindMessage : IMessage
{
    public readonly Vector2 Target;
    public readonly bool ConsiderNearbySolids;
    public RecalculatePathfindMessage(Vector2 target, bool considerNearbySolids)
    {
        ConsiderNearbySolids = considerNearbySolids;
        Target = target;
    }
}
