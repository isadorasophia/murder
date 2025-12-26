using Bang;
using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Text.Json.Serialization;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct OverrideYOffsetComponent : IComponent
{
    public readonly float YOffset = 0;

    public OverrideYOffsetComponent() { }
}
