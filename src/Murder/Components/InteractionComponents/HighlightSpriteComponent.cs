using Bang.Components;
using Murder.Core.Graphics;

namespace Murder.Components
{
    public readonly struct HighlightSpriteComponent : IComponent
    {
        public readonly Color Color = Color.White;

        public HighlightSpriteComponent(Color color)
        {
            Color = color;
        }
    }
}
