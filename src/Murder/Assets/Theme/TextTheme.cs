using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Assets;

public readonly struct TextTheme
{
    [PaletteColor]
    public readonly Color? Color = null;

    [PaletteColor]
    public readonly Color? Outline = null;

    [PaletteColor]
    public readonly Color? Shadow = null;

    public TextTheme() { }

    public bool Empty => Color is null && Outline is null && Shadow is null;
}
