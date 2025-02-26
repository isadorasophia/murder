using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[Flags]
public enum PathfindStatusFlags
{
    None = 0,
    PathNotFound = 1 << 0,
    HasLineOfSight = 1 << 1,
    PathfindSuccessul = 1 << 2,
    PathComplete = 1 << 3,
}

[RuntimeOnly]
public readonly struct PathfindStatusComponent : IComponent
{
    public readonly PathfindStatusFlags Flags = PathfindStatusFlags.None;

    public PathfindStatusComponent(PathfindStatusFlags status)
    {
        Flags = status;
    }
}
