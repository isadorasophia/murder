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

        public void Subscribe(Action _) { }

        public void Unsubscribe(Action _) { }
        public EditorComponent()
        {
            EditorHook = new EditorHook(true);
        }
        public EditorComponent(EditorHook hook) => EditorHook = hook;
        public EditorComponent(bool playMode)
        {
            EditorHook = new EditorHook(playMode);
        }
    }
}