using Microsoft.Xna.Framework;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Murder.Core.Graphics
{
    public struct Color : IEquatable<Color>
    {
        private static readonly Color _white = new(1, 1, 1);
        private static readonly Color _gray = new(0.5f, 0.5f, 0.5f);
        private static readonly Color _black = new(0, 0, 0);
        private static readonly Color _coldGray = new(0.45f, 0.5f, 0.55f);
        private static readonly Color _brightGray = new(0.65f, 0.75f, 0.75f);
        private static readonly Color _warmGray = new(0.55f, 0.5f, 0.45f);
        private static readonly Color _transparent = new(0, 0, 0, 0);
        private static readonly Color _blue = new(0, 0, 1);
        private static readonly Color _green = new(0, 1, 0);
        private static readonly Color _orange= new(1, 0.6f, 0.1f);
        private static readonly Color _red = new(1, 0, 0);
        private static readonly Color _magenta = new(1, 0, 1);
        public static Color White => _white;
        public static Color Gray => _gray;
        public static Color Black => _black;
        public static Color BrightGray => _brightGray;
        public static Color ColdGray => _coldGray;
        public static Color WarmGray => _warmGray;
        public static Color Transparent => _transparent;
        public static Color Red => _red;
        public static Color Blue => _blue;
        public static Color Green => _green;
        public static Color Orange => _orange;
        public static Color Magenta => _magenta;
        public float R = 0;
        public float G = 0;
        public float B = 0;
        public float A = 0;

        public Color(float r, float g, float b) : this(r, g, b, 1) { }
        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public Color FadeAlpha(float alpha) => new(R, G, B, A * alpha);

        public static Color CreateFrom256(int r, int g, int b) =>
            new Color(r / 256f, g / 256f, b / 256f, 1f);

        public static implicit operator Microsoft.Xna.Framework.Color(Color c) => new(c.R, c.G, c.B, c.A);
        public static implicit operator Color(Microsoft.Xna.Framework.Color c) => new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        public static implicit operator Color(System.Numerics.Vector4 c) => new(c.X, c.Y, c.Z, c.W);
        
        //public static Color operator *(Color c, float factor) => new(c.R * factor, c.G * factor, c.B * factor, c.A * factor);
        
        public static implicit operator uint(Color c) { uint ret = (uint)(c.A * 255); ret <<= 8; ret += (uint)(c.B * 255); ret <<= 8; ret += (uint)(c.G * 255); ret <<= 8; ret += (uint)(c.R * 255); return ret; }

        public Color Darken(float r) => new(R * r, G * r, B * r, A);
        public static Color operator *(Color l, float r) => new(l.R * r, l.G * r, l.B * r, l.A * r);

        public static Color Parse(String str)
        {
            string pattern = @"Color\((?<r>[\d.]+),\s*(?<g>[\d.]+),\s*(?<b>[\d.]+),\s*(?<a>[\d.]+)\)";
            
            Match match = Regex.Match(str, pattern);

            if (match.Success)
            {
                float r = float.Parse(match.Groups["r"].Value);
                float g = float.Parse(match.Groups["g"].Value);
                float b = float.Parse(match.Groups["b"].Value);
                float a = float.Parse(match.Groups["a"].Value);

                Console.WriteLine($"r: {r}, g: {g}, b: {b}, a: {a}");
                return new Color(r, g, b, a);
            }
            else
            {
                Console.WriteLine("No match found.");
                return Color.Magenta;
            }
        }
        public override string ToString()
        {
            return $"Color({R}, {G}, {B}, {A})";
        }

        public Color Premultiply()
        {
            return new Color(
                (R * A),
                (G * A),
                (B * A),
                A
            );
        }

        public static Color Lerp(Color a, Color b, float factor)
        {
            return new(
                Calculator.Lerp(a.R, b.R, factor),
                Calculator.Lerp(a.G, b.G, factor),
                Calculator.Lerp(a.B, b.B, factor),
                Calculator.Lerp(a.A, b.A, factor)
            );
        }

        /// <summary>
        /// Parses a string <paramref name="hex"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="hex">The string as the hex value, e.g. "#ff5545". Alpha will always be 1.</param>
        /// <returns>The converted color.</returns>
        public static Color FromHex(string hex)
        {
            // Parse the hexadecimal string into an integer
            int hexValue = int.Parse(hex.TrimStart('#'), NumberStyles.HexNumber);

            // Extract the red, green, and blue color values from the integer
            int r = (hexValue >> 16) & 0xFF;
            int g = (hexValue >> 8) & 0xFF;
            int b = hexValue & 0xFF;

            // Normalize the color values to the range of 0 to 1
            float normalize = 1.0f / 255.0f;
            float rf = r * normalize;
            float gf = g * normalize;
            float bf = b * normalize;

            // Create a new Color object with the normalized color values
            return new Color(rf, gf, bf);
        }


        private static readonly Regex _colorRegex = new Regex(@"\$?Color\(([\d.]+), ([\d.]+), ([\d.]+), ([\d.]+)\)");

        internal static Color FromName(string value)
        {
            Match match = _colorRegex.Match(value);
            if (match.Success)
            {
                float r = float.Parse(match.Groups[1].Value);
                float g = float.Parse(match.Groups[2].Value);
                float b = float.Parse(match.Groups[3].Value);
                float a = float.Parse(match.Groups[4].Value);
                return new Color(r, g, b, a);
            }

            try
            {
                return FromHex(value);
            }
            catch
            {
                GameLogger.Fail($"Invalid input: {value}.");
                return Color.White;
            }
        }

        public System.Numerics.Vector4 ToSysVector4()
        {
            return new(this.R, this.G, this.B, this.A);
        }
    }
}