using Murder.Diagnostics;
using Murder.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// The color type as described by the engine. Values are represented as <see cref="float"/> from 0 to 1.
    /// To create a color using 0-255, use <see cref="CreateFrom256(byte,byte,byte,byte)"/>.
    /// </summary>
    public readonly partial struct Color : IEquatable<Color>
    {
        /// <summary>
        /// Opaque color with 0 red, green or blue.
        /// </summary>
        public static Color Black { get; } = new(0, 0, 0);
        
        /// <summary>
        /// Opaque color with max red, green and blue.
        /// </summary>
        public static Color White { get; } = new(1, 1, 1);
        
        /// <summary>
        /// Opaque color with 50% red, green and blue.
        /// </summary>
        public static Color Gray { get; } = new(0.5f, 0.5f, 0.5f);
        
        /// <summary>
        /// Opaque color with 65% red, 75% green and 75% blue.
        /// </summary>
        public static Color BrightGray { get; } = new(0.65f, 0.75f, 0.75f);

        /// <summary>
        /// Like <see cref="Gray"/> but with a blue-ish tint.
        /// </summary>

        public static Color ColdGray { get; } = new(0.45f, 0.5f, 0.55f);
        
        /// <summary>
        /// Like <see cref="Gray"/> but with a red-ish tint.
        /// </summary>
        public static Color WarmGray { get; } = new(0.55f, 0.5f, 0.45f);
        
        /// <summary>
        /// <see cref="Black"/> but with 0 alpha.
        /// </summary>
        public static Color Transparent { get; } = new(0, 0, 0, 0);
        
        /// <summary>
        /// Pure red (no green or blue).
        /// </summary>
        public static Color Red { get; } = new(1, 0, 0);
        
        /// <summary>
        /// A shade of orange.
        /// </summary>
        public static Color Orange { get; } = new(1, 0.6f, 0.1f);
        
        /// <summary>
        /// Pure yellow (max red and green, no blue).
        /// </summary>
        public static Color Yellow { get; } = new(1, 1, 0);
        
        /// <summary>
        /// Pure green (no red or blue).
        /// </summary>
        public static Color Green { get; } = new(0, 1, 0);
        
        /// <summary>
        /// Pure cyan (max green and blue, no red).
        /// </summary>
        public static Color Cyan { get; } = new(0, 1, 1);
        
        /// <summary>
        /// Pure blue (no red or green).
        /// </summary>
        public static Color Blue { get; } = new(0, 0, 1);
        
        /// <summary>
        /// Pure magenta (max red and blue, no green).
        /// </summary>
        public static Color Magenta { get; } = new(1, 0, 1);

        /// <summary>
        /// Amount of red in this color.
        /// </summary>
        public readonly float R = 0;

        /// <summary>
        /// Amount of green in this color.
        /// </summary>
        public readonly float G = 0;
        
        /// <summary>
        /// Amount of blue in this color.
        /// </summary>
        public readonly float B = 0;
        
        /// <summary>
        /// Transparency of this color.
        /// </summary>
        public readonly float A = 0;

        /// <summary>
        /// Creates a color with the specified values. If the fourth
        /// argument is omitted, the value used for the alpha will be 1,
        /// meaning a completely opaque color.
        /// Do note colors in Murder use 0-1 as their range.
        /// To initialize a color using 0-255, please refer to <see cref="CreateFrom256(byte,byte,byte,byte)"/>.
        /// </summary>
        /// <param name="r">Amount of red in the color.</param>
        /// <param name="g">Amount of green in the color.</param>
        /// <param name="b">Amount of blue in the color.</param>
        /// <param name="a">How transparent the color will be.</param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        /// <summary>
        /// Keeps all color values equal except for the alpha which is multiplied by <see cref="factor"/>
        /// </summary>
        /// <param name="factor">By how much the alpha of this color will be multiplied.</param>
        /// <returns>A new color with the modified alpha</returns>
        public Color FadeAlpha(float factor) => new(R, G, B, A * factor);

        /// <summary>
        /// Creates a color using values from 0 to 255.
        /// </summary>
        public static Color CreateFrom256(byte r, byte g, byte b) =>
            new(r / 256f, g / 256f, b / 256f);

        
        /// <summary>
        /// Creates a color using values from 0 to 255.
        /// </summary>
        public static Color CreateFrom256(byte r, byte g, byte b, byte a) =>
            new(r / 256f, g / 256f, b / 256f, a / 256f);

        /// <summary>
        /// Converts the murder color <see cref="c"/> into an XNA color.
        /// </summary>
        public static implicit operator Microsoft.Xna.Framework.Color(Color c) => new(c.R, c.G, c.B, c.A);
        
        /// <summary>
        /// Converts the XNA color <see cref="c"/> into a murder color.
        /// </summary>
        public static implicit operator Color(Microsoft.Xna.Framework.Color c) => new(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        
        /// <summary>
        /// Converts the Vector4 into a murder color (X = R, Y = G, Z = B, W = A).
        /// </summary>
        public static implicit operator Color(System.Numerics.Vector4 c) => new(c.X, c.Y, c.Z, c.W);

        /// <summary>
        /// Converts this color in its unsigned integer representation
        /// </summary>
        public static explicit operator uint(Color c) { uint ret = (uint)(c.A * 255); ret <<= 8; ret += (uint)(c.B * 255); ret <<= 8; ret += (uint)(c.G * 255); ret <<= 8; ret += (uint)(c.R * 255); return ret; }

        /// <summary>
        /// Multiplies all values of the color by the float <see cref="r"/>
        /// </summary>
        public static Color operator *(Color l, float r) => new(l.R * r, l.G * r, l.B * r, l.A * r);

        /// <summary>
        /// Multiplies each color value in color <see cref="l"/> by their correspondent counterpart in color <see cref="r"/>
        /// </summary>
        public static Color operator *(Color l, Color r) => new(l.R * r.R, l.G * r.G, l.B * r.B, l.A * r.A);
        
        /// <summary>
        /// Checks if two colors are equal.
        /// </summary>
        public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;

        /// <inheritdoc cref="obj"/>
        public override bool Equals(object? obj) => obj is Color other && Equals(other);

        /// <inheritdoc cref="Object"/>
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

        /// <inheritdoc cref="Object"/>
        public static bool operator ==(Color lhs, Color rhs) => lhs.Equals(rhs);

        /// <inheritdoc cref="Object"/>
        public static bool operator !=(Color lhs, Color rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tries to interpret a color from a string. Returns <see cref="Magenta"/> in case of failure.
        /// </summary>
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

            Console.WriteLine("No match found.");
            return Magenta;
        }
        
        /// <inheritdoc cref="Object"/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Color({0}, {1}, {2}, {3})", R, G, B, A);
        }
        
        /// <summary>
        /// Multiplies all values except the alpha by the factor <see cref="r"/>.
        /// While this is named "Darken", values above 1.0f will effectively make the color lighter.
        /// </summary>
        public Color Darken(float r) => new(R * r, G * r, B * r, A);

        /// <summary>
        /// Multiplies the R, G and B values of this color by the Alpha value.
        /// </summary>
        public Color Premultiply()
        {
            return new Color(
                (R * A),
                (G * A),
                (B * A),
                A
            );
        }

        /// <summary>
        /// Finds a color that is in the point <see cref="factor"/> between <see cref="a"/> and <see cref="b"/>.
        /// </summary>
        public static Color Lerp(Color a, Color b, [Range(0, 1)] float factor)
            => new(
                Calculator.Lerp(a.R, b.R, factor),
                Calculator.Lerp(a.G, b.G, factor),
                Calculator.Lerp(a.B, b.B, factor),
                Calculator.Lerp(a.A, b.A, factor)
            );

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

        [GeneratedRegex(@"\$?Color\(([\d.]+), ([\d.]+), ([\d.]+), ([\d.]+)\)")]
        private static partial Regex ColorRegex();

        internal static Color FromName(string value)
        {
            Match match = ColorRegex().Match(value);
            if (match.Success)
            {
                float r = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                float g = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                float b = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                float a = float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Converts this color into a <see cref="System.Numerics.Vector4"/> with X = R, Y = G, Z = B and W = A.
        /// </summary>
        public System.Numerics.Vector4 ToSysVector4()
            => new(this.R, this.G, this.B, this.A);

        /// <summary>
        /// Interprets the Vector4 <see cref="color"/> as a color and return the unsigned integer representation of that color.
        /// </summary>
        public static uint ToUint(System.Numerics.Vector4 color)
            => (uint)(new Color(color.X,color.Y, color.Z, color.W));
    }
}