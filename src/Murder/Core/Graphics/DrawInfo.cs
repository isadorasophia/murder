using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Core.Graphics;

public enum BlendStyle
{
    Normal,
    Wash,
    Color
}

[Flags]
public enum OutlineStyle
{
    None       = 1 << 0,
    TopOnly    = 1 << 1,
    RightOnly  = 1 << 2,
    BottomOnly = 1 << 3,
    LeftOnly   = 1 << 4,
    Top        = TopOnly | RightOnly | LeftOnly,
    Right      = TopOnly | BottomOnly | LeftOnly,
    Bottom     = RightOnly | BottomOnly | LeftOnly,
    Left       = TopOnly | BottomOnly | LeftOnly,
    Full       = TopOnly | RightOnly | BottomOnly | LeftOnly,
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
    public bool FlippedHorizontal { get; init; } = false;

    public Rectangle Clip { get; init; } = Rectangle.Empty;

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
        switch (BlendMode)
        {
            case BlendStyle.Normal: return new(1, 0, 0);
            case BlendStyle.Wash: return new(0, 1, 0);
            case BlendStyle.Color: return new(0, 0, 1);
            default:
                throw new Exception("Blend mode not supported!");
        }
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
            FlippedHorizontal = FlippedHorizontal,
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
            FlippedHorizontal = FlippedHorizontal,
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
        FlippedHorizontal = FlippedHorizontal,
        Debug = Debug
    };

}