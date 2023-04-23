using Bang.Components;

namespace Murder.Components;

/// <summary>
/// Ignores all trigger collisions until a time is reached, then it gets removed
/// </summary>
public readonly struct IgnoreTriggersUntilComponent : IComponent
{
    public readonly float Until = 0;

    public IgnoreTriggersUntilComponent(float until)
    {
        Until = until;
    }
}