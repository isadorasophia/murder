using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Services;

public struct DrawMenuStyle
{
    public Vector2 Origin = new(0.5f, 0);

    [Font]
    public int Font = (int)MurderFonts.LargeFont;

    [PaletteColor]
    public Color SelectedColor = Color.White;
    [PaletteColor]
    public Color Color = Color.ColdGray;
    [PaletteColor]
    public Color Shadow = Color.Black;

    public EaseKind Ease;
    public float SelectorMoveTime;

    public int ExtraVerticalSpace = 2;

    public DrawMenuStyle()
    {
    }

    public DrawMenuStyle WithOrigin(Vector2 origin)
    {
        return new DrawMenuStyle() with
        {
            Origin = origin,
            Font = Font,
            SelectedColor = SelectedColor,
            Color = Color,
            Shadow = Shadow,
            Ease = Ease,
            SelectorMoveTime = SelectorMoveTime,
            ExtraVerticalSpace = ExtraVerticalSpace
        };
    }
}