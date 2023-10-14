using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Unique, DoNotPersistOnSave, RuntimeOnly]
    public readonly struct FreezeWorldComponent : IComponent
    {
        public readonly int Count = 1;

        public FreezeWorldComponent() { }

        public FreezeWorldComponent(int count) => Count = count;
    }
}