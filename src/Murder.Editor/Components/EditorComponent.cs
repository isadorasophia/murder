using Bang.Components;
using Murder.Attributes;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Editor.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public struct EditorComponent : IModifiableComponent
    {
        public readonly EditorHook EditorHook = new();

        public EditorComponent() { }

        public void Subscribe(Action notification) { }

        public void Unsubscribe(Action notification) { }
    }
}