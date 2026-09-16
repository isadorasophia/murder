using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components.Graphics;

public readonly struct TintComponent : IComponent
{
    [ShowInEditor, PaletteColor]
    public readonly Color TintColor;

    public TintComponent(Color color)
    {
        TintColor = color;
    }
}