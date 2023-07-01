
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.Immutable;
using System.Drawing;
using System.Text.Json;

namespace Murder.Editor.Data.Graphics;

internal class FontImporter
{
    public static void GenerateFont(int fontIndex, string fontPath, int fontSize, string outputPath)
    {
        string pngPath = outputPath + ".png";

        using (var stream = new SKFileWStream(pngPath))
        using (var skPaint = new SKPaint())
        {
            skPaint.IsAntialias = false;
            skPaint.TextSize = fontSize;
            skPaint.Typeface = SKTypeface.FromFile(fontPath);
            skPaint.Color = new SKColor(255, 255, 255);
            skPaint.TextAlign = SKTextAlign.Left;

            var bounds = new SKRect();
            var characters = new Dictionary<int, PixelFontCharacter>();
            var maxWidth = 0;
            var nextPosition = 0;

            // Get the font metrics
            SKFontMetrics fontMetrics = new SKFontMetrics();
            skPaint.GetFontMetrics(out fontMetrics);

            // Measure each character and store in dictionary
            for (int i = 32; i < 127; i++)
            {
                string charAsString = ((char)i).ToString();
                var adjust = skPaint.MeasureText(charAsString, ref bounds);
                var advance = skPaint.GetGlyphWidths(charAsString).FirstOrDefault();
                var offset = skPaint.GetGlyphPositions(charAsString).FirstOrDefault();
                Point size = new((int)bounds.Width, (int)bounds.Height);
                
                var character = new PixelFontCharacter(i,
                    new(nextPosition, 0, size.X, size.Y),
                    (int)bounds.Left, (int)bounds.Top, Calculator.CeilToInt(advance));
                characters[i] = character;
                maxWidth = Math.Max(maxWidth, (int)bounds.Width);
                nextPosition += size.X + 1;
            }

            // Calculate kerning for each pair of characters
            var kernings = new List<Kerning>();
            for (int i = 32; i < 127; i++)
            {
                for (int j = 32; j < 127; j++)
                {
                    ushort[] pair = new ushort[] { (ushort)i, (ushort)j };
                    var adjustments = skPaint.Typeface.GetKerningPairAdjustments(pair);
                    if (adjustments.Length > 0)
                    {
                        kernings.Add(new Kerning { First = i, Second = j, Amount = adjustments[0] });
                    }
                }
            }


            using (var bitmap = new SKBitmap(nextPosition, nextPosition, SKImageInfo.PlatformColorType, SKAlphaType.Premul))
            using (var canvas = new SKCanvas(bitmap))
            {
                // Draw each character onto the bitmap
                foreach (var character in characters)
                {
                    var glyph = character.Value.Glyph;

                    //skPaint.Color = new SKColor(255, 155, 0);
                    //canvas.DrawRect(new SKRect((int)glyph.Left, (int)glyph.Top, (int)glyph.Right, (int)glyph.Bottom),
                    //   skPaint);
                    //skPaint.Color = new SKColor(255, 255, 255);
                    canvas.DrawText(((char)character.Key).ToString(),
                        glyph.Left - character.Value.XOffset, glyph.Top - character.Value.YOffset,
                        skPaint);
                }

                // Save bitmap to PNG
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            var fontAsset = new FontAsset(fontIndex, characters, kernings.ToImmutableArray(), fontSize, fontPath, -fontMetrics.Ascent);

            // Save characters to JSON
            FileHelper.SaveSerialized(fontAsset, outputPath + ".json", false);
        }
    }

}
