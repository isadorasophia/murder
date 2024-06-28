using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Editor.Components
{
    [DoNotPersistOnSave]
    public readonly struct DebugColorComponent : IComponent
    {
        public readonly Color Color;

        public DebugColorComponent(Color color)
        {
            Color = color;
        }
    }
}