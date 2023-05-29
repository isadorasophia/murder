using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    [DoNotPersistEntityOnSave]
    public readonly struct DoNotPersistEntityOnSaveComponent : IComponent { }
}
