using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

/// <summary>
/// Whether something is currently being hidden (say, by a roof).
/// </summary>
[RuntimeOnly]
public readonly struct HiddenStatusComponent : IComponent
{
    /// <summary>
    /// If true, this means that this is hidden by a sprite (say, a roof).
    /// </summary>
    public readonly bool Hidden = true;

    public HiddenStatusComponent() { }

    public HiddenStatusComponent(bool hidden) => Hidden = hidden;
}
