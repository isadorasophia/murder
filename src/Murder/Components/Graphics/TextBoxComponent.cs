using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public readonly struct TextBoxComponent : IComponent
    {
        public readonly string Text = string.Empty;
        public readonly float FontSize = 1;

        [Slider]
        public readonly float Sorting = 0;
        public readonly int VisibleCharacters;
        
        [Tooltip("(0,0) is top left and (1,1) is bottom right"), Slider]
        public readonly Vector2 Offset;
        [PaletteColor]
        public readonly Color Color;

        public TextBoxComponent(string text, int visibleCharacters, float fontSize, float sorting, Color color, Vector2 offset)
        {
            Text = text;
            VisibleCharacters = visibleCharacters;
            Sorting = sorting;
            FontSize = fontSize;
            Color = color;
            Offset = offset;
        }

        public TextBoxComponent WithVisibleCharacters(int visibleCaracters) =>
            new(Text, visibleCaracters, FontSize, Sorting, Color, Offset);

        public TextBoxComponent WithText(string text) =>
            new(text, visibleCharacters: 0, FontSize, Sorting, Color, Offset);
    }
}
