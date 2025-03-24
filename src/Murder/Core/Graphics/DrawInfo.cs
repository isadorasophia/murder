using Murder.Core.Geometry;
using Murder.Services;
using System.Numerics;

namespace Murder.Core.Graphics;

public enum BlendStyle
{
    Normal,
    Wash,
    Color
}
public enum MurderBlendState
{
    AlphaBlend,
    Additive,
}

[Flags]
public enum OutlineStyle
{
    None   = 0,
    Top    = 1 << 0,
    Right  = 1 << 1,
    Bottom = 1 << 2,
    Left   = 1 << 3,
    Full   = Top | Right | Bottom | Left,
}

/// <summary>
/// Generic struct for drawing things without cluttering methods full of arguments.
/// Note that not all fields are supported by all methods.
/// Tip: Create a new one like this: <code>new DrawInfo(){ Color = Color.Red, Sort = 0.2f}</code>
/// </summary> 
public readonly struct DrawInfo
{
    public static DrawInfo Default => new();

    /// <summary>
    /// The origin of the image. From 0 to 1. Vector2Helper.Center is the center.
    /// </summary>
    public Vector2 Origin { get; init; } = Vector2.Zero;

    /// <summary>
    /// An offset to draw this image. In pixels
    /// </summary>
    public Vector2 Offset { get; init; } = Vector2.Zero;

    /// <summary>
    /// In degrees.
    /// </summary>
    public float Rotation { get; init; } = 0;
    public Vector2 Scale { get; init; } = Vector2.One;
    public Color Color { get; init; } = Color.White;
    public float Sort { get; init; } = 0.5f;

    public OutlineStyle OutlineStyle { get; init; } = OutlineStyle.Full;
    public Color? Outline { get; init; } = null;
    public Color? Shadow { get; init; } = null;
    public bool Debug { get; init; } = false;

    public BlendStyle BlendMode { get; init; } = BlendStyle.Normal;
    public MurderBlendState BlendState { get; init; } = MurderBlendState.AlphaBlend;

    public ImageFlip ImageFlip { get; init; } = ImageFlip.None;

    public Rectangle Clip { get; init; } = Rectangle.Empty;

    /// <summary>
    /// Whether this should bypass localization fonts.
    /// </summary>
    public bool CultureInvariant { get; init; } = false;

    public DrawInfo()
    {
    }
    public DrawInfo(Color color, float sort)
    {
        Color = color;
        Sort = sort;
    }

    public DrawInfo(float sort)
    {
        Sort = sort;
    }

    public Microsoft.Xna.Framework.Vector3 GetBlendMode()
    {
        return RenderServices.ToVector3(BlendMode);
    }

    internal DrawInfo WithScale(Vector2 size)
    {
        return new DrawInfo()
        {
            Origin = Origin,
            Rotation = Rotation,
            Color = Color,
            Sort = Sort,
            Scale = Scale * size,
            Offset = Offset,
            Shadow = Shadow,
            Outline = Outline,
            BlendMode = BlendMode,
            ImageFlip = ImageFlip,
            Debug = Debug
        };
    }

    internal DrawInfo WithOffset(Vector2 offset)
    {
        return new DrawInfo()
        {
            Rotation = Rotation,
            Color = Color,
            Sort = Sort,
            Scale = Scale,
            Origin = Origin,
            Offset = offset,
            Shadow = Shadow,
            Outline = Outline,
            BlendMode = BlendMode,
            ImageFlip = ImageFlip,
            Debug = Debug
        };
    }

    public DrawInfo WithSort(float sort) => new DrawInfo()
    {
        Rotation = Rotation,
        Color = Color,
        Sort = sort,
        Scale = Scale,
        Origin = Origin,
        Offset = Offset,
        Shadow = Shadow,
        Outline = Outline,
        BlendMode = BlendMode,
        ImageFlip = ImageFlip,
        Debug = Debug
    };

}