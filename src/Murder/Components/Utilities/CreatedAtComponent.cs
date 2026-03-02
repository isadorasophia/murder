using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct CreatedAtComponent : IComponent
{
    [Tooltip("Time that this entity was created, in seconds")]
    public readonly float When { get; init; }

    public CreatedAtComponent(float when)
    {
        When = when;
    }
}
