using Bang.Components;
using Murder.Core.Graphics;

namespace Murder.Editor.Components
{
    public readonly struct DebugColorComponent : IComponent
    {
        public readonly Color Color;

        public DebugColorComponent(Color color)
        {
            Color = color;
        }
    }
}