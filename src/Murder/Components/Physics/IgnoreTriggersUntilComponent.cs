using Bang.Components;
using Murder.Attributes;

namespace Murder.Components;

/// <summary>
/// Ignores all trigger collisions until a time is reached, then it gets removed
/// </summary>
[DoNotPersistOnSave]
public readonly struct IgnoreTriggersUntilComponent : IComponent
{
    public readonly float Until = 0;

    public IgnoreTriggersUntilComponent(float until)
    {
        Until = until;
    }
}