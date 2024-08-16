using Bang.Components;
using Murder.Attributes;

namespace Murder.Components;

/// <summary>
/// Useful for tagging an entity for some systems until X time
/// </summary>
[DoNotPersistOnSave]
[KeepOnReplace]
public readonly struct IgnoreUntilComponent : IComponent
{
    /// <summary>
    /// When to remove this component. A negative value will never be automatically removed.
    /// </summary>
    public readonly float Until = 0;

    public IgnoreUntilComponent(float until)
    {
        Until = until;
    }
}
