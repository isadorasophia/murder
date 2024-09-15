using Bang.Components;
using Murder.Attributes;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct MoveToMaxTimeComponent : IComponent
{
    public readonly float RemoveAt;

    public MoveToMaxTimeComponent(float removeAt)
    {
        RemoveAt = removeAt;
    }
}
