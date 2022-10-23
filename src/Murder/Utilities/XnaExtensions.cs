using Murder.Core.Geometry;

using SystemVector4 = System.Numerics.Vector4;
using XnaVector4 = Microsoft.Xna.Framework.Vector4;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Murder.Utilities
{
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

        public static Point ToPoint(this Vector2 vector2) =>
            new(Calculator.RoundToInt(vector2.X), Calculator.RoundToInt(vector2.Y));

        public static Rectangle ToRectangle(float x, float y, float width, float height)
        {
            int ix = Calculator.RoundToInt(x);
            int iy = Calculator.RoundToInt(y);
            int iwidth = Calculator.RoundToInt(width);
            int iheight = Calculator.RoundToInt(height);

            return new Rectangle(ix, iy, iwidth, iheight);
        }
    }
}