using Bang.Components;
using Murder.Attributes;

namespace Murder.Components.Graphics
{
    [DoNotPersistEntityOnSave]
    public readonly struct UiDisplayComponent : IComponent
    {
        public readonly float YSort = 0;

        public UiDisplayComponent() { }
    }
}
