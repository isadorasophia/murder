using System.Drawing;
using System.Numerics;

namespace Murder.Utilities
{
    public static class ColorHelper
    {
        /// <summary>
        /// Parses a string <paramref name="hex"/> to <see cref="Vector4"/>.
        /// </summary>
        /// <param name="hex">The string as the hex value, e.g. "#ff5545".</param>
        /// <returns>The converted color.</returns>
        public static Vector4 ToVector4Color(this string hex) {
            var rgba = ColorTranslator.FromHtml(hex);
            return new Vector4(rgba.R/256f, rgba.G/256f, rgba.B/256f, 1);
        }
    }
}