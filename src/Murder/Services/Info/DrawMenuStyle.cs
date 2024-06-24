using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Services;

public readonly struct DrawMenuStyle
{
    public Vector2 Origin { get; init; } = new(0.5f, 0);

    [Font]
    public int Font { get; init; } = (int)MurderFonts.LargeFont;

    [PaletteColor]
    public Color SelectedColor { get; init; } = Color.White;
    [PaletteColor]
    public Color Color { get; init; } = Color.ColdGray;
    [PaletteColor]
    public Color Shadow { get; init; } = Color.Black;

    public EaseKind Ease { get; init; }
    public float SelectorMoveTime { get; init; }

    public int ExtraVerticalSpace { get; init; } = 2;

    public DrawMenuStyle()
    {
    }

}