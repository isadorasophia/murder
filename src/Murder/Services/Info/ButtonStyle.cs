

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
    public readonly Portrait Sprite { get; init; } = new();
    [PaletteColor]
    public readonly Color? OutlineColor { get; init; } = null;

    [Font]
    public readonly int Font;
    [Slider(0,1)]
    public readonly float TextAlignment { get; init; } = 0;
    [PaletteColor]
    public readonly Color TextColor { get; init; } = Color.White;
    [PaletteColor]
    public readonly Color? TextShadowColor { get; init; } = null;
    [PaletteColor]
    public readonly Color? TextOutlineColor { get; init; } = null;

    public readonly Point ExtraPaddingX { get; init; }
}
