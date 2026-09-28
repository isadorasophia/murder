using Bang.Components;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public readonly struct HighlightSpriteComponent : IComponent
    {
        [PaletteColor]
        public readonly Color Color = Color.White;

        public HighlightSpriteComponent(Color color)
        {
            Color = color;
        }
    }
}