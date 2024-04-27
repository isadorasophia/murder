using Murder.Core.Geometry;
using System.Numerics;
using SystemVector4 = System.Numerics.Vector4;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaVector4 = Microsoft.Xna.Framework.Vector4;

namespace Murder.Utilities;

public static class XnaExtensions
{
    public static SystemVector4 ToSysVector4(this XnaColor color)
    {
        // Use XNA converter.
        XnaVector4 vec4 = color.ToVector4();
        return new SystemVector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
    }

    public static XnaColor ToXnaColor(this SystemVector4 color)
    {
        return new XnaColor(color.X, color.Y, color.Z, color.W);
    }

    public static Point ToXnaPoint(this Vector2 vector2) =>
        new(Calculator.RoundToInt(vector2.X), Calculator.RoundToInt(vector2.Y));

    public static Rectangle ToRectangle(float x, float y, float width, float height)
    {
        int ix = Calculator.RoundToInt(x);
        int iy = Calculator.RoundToInt(y);
        int iwidth = Calculator.RoundToInt(width);
        int iheight = Calculator.RoundToInt(height);

        return new Rectangle(ix, iy, iwidth, iheight);
    }

    public static Microsoft.Xna.Framework.Vector2 ToVector2(Vector2 v) =>
        new Microsoft.Xna.Framework.Vector2(v.X, v.Y);

    public static Point Size(this Microsoft.Xna.Framework.Rectangle @this) =>
        new(@this.Width, @this.Height);

    public static Microsoft.Xna.Framework.Vector2 XnaSize(this Microsoft.Xna.Framework.Rectangle @this) =>
        new(@this.Width, @this.Height);

    public static Point ToPoint(this Vector2 vector) => new(Calculator.RoundToInt(vector.X), Calculator.RoundToInt(vector.Y));
    public static Vector2 ToSysVector2(this Microsoft.Xna.Framework.Point point) => new(point.X, point.Y);
    public static Vector2 ToSysVector2(this Microsoft.Xna.Framework.Vector2 vector) => new(vector.X, vector.Y);
    public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this Vector2 vector) => new(vector.X, vector.Y);
    public static Vector2 ToCore(this Vector2 vector) => new(vector.X, vector.Y);
}
