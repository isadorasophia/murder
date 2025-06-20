using Murder.Core.Graphics;
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
        public static Vector4 ToVector4Color(this string hex)
        {
            var rgba = System.Drawing.ColorTranslator.FromHtml(hex);
            return new Vector4(rgba.R / 256f, rgba.G / 256f, rgba.B / 256f, 1);
        }

        public static Color MultiplyAlpha(this Color color)
        {
            return new Color(color.R * color.A, color.G * color.A, color.B * color.A, color.A);
        }

        /// <summary>
        /// Generates a unique, visually distinct color based on the index using HSV color space.
        /// </summary>
        public static Color ColorFromIndex(int index, int total, float saturation = 0.6f, float value = 0.9f)
        {
            float hue = (index % total) / (float)total;
            return ColorFromHSV(hue * 360f, saturation, value);
        }

        /// <summary>
        /// Converts HSV to RGB. Hue is in [0, 360], saturation and value are in [0, 1].
        /// </summary>
        public static Color ColorFromHSV(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - MathF.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float r, g, b;
            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return new Color(r + m, g + m, b + m, 1);
        }
    }
}