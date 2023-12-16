using Bang.Components;
using Murder.Attributes;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Editor.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    internal class TimelineEditorComponent : IComponent
    {
        public readonly TimelineEditorHook Hook = new();

        public TimelineEditorComponent() { }

        public void Subscribe(Action _) { }

        public void Unsubscribe(Action _) { }
    }
}
