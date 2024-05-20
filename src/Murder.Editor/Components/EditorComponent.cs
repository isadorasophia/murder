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
        public readonly EditorHook EditorHook;

        public EditorComponent() => EditorHook = new();

        public EditorComponent(EditorHook? hook) => EditorHook = hook ?? new();

        public void Subscribe(Action _) { }

        public void Unsubscribe(Action _) { }
    }
}