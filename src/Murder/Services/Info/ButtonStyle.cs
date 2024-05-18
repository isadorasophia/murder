

using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Services;

public readonly struct ButtonStyle()
{
    public readonly Portrait Sprite = new();
    [PaletteColor]
    public readonly Color? OutlineColor = null;

    [Font]
    public readonly int Font;
    [Slider(0,1)]
    public readonly float TextAlignment = 0;
    [PaletteColor]
    public readonly Color TextColor = Color.White;
    [PaletteColor]
    public readonly Color? TextShadowColor = null;
    [PaletteColor]
    public readonly Color? TextOutlineColor = null;

    public readonly Point ExtraPaddingX { get; init; }
}
