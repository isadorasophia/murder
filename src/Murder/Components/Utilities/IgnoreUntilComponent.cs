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

    /// <summary>
    /// Whether we allow to interact with this. This is sometimes the case when we want to stop
    /// displaying, but still give a feedback that it was interacted.
    /// </summary>
    public readonly bool AllowInteract = false;

    public IgnoreUntilComponent(float until)
    {
        Until = until;
    }

    public IgnoreUntilComponent(bool allowInteract, float until) : this(until)
    {
        AllowInteract = allowInteract;
    }
}
