using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    [Unique, DoNotPersistOnSave]
    public readonly struct FreezeWorldComponent : IComponent
    {
    }
}
