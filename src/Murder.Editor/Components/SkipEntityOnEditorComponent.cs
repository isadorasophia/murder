using Bang.Components;
using Murder.Attributes;

namespace Murder.Editor.Components
{
    /// <summary>
    /// Used to skip entities on systems.
    /// </summary>
    [DoNotPersistOnSave]
    internal readonly struct SkipEntityOnEditorComponent : IComponent
    {
    }
}
