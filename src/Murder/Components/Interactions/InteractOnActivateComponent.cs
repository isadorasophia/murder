using Bang.Components;

namespace Murder.Components;

public readonly struct InteractOnActivateComponent() : IComponent
{
    [Flags]
    public enum InteractOnActivateFlags
    {
        None = 0,
        InteractOnAdded = 1 << 0,
    }

    public readonly InteractOnActivateFlags Flags = InteractOnActivateFlags.None;

}
