using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Services;

public struct DrawMenuStyle
{
    public Vector2 Origin = new Vector2(0.5f, 0);

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

    public DrawMenuStyle()
    {
    }
}