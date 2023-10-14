using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Serialization;
using Murder.Utilities;
using SkiaSharp;
using System.Collections.Immutable;
using System.Drawing;

namespace Murder.Editor.Data.Graphics;

internal class FontImporter
{
    public static string SourcePackedPath => FileHelper.GetPath(Architect.EditorSettings.SourcePackedPath, Game.Profile.FontsPath);

    public static bool GenerateFontJsonAndPng(int fontIndex, string fontPath, int fontSize, string name)
    {
        string sourcePackedPath = SourcePackedPath;
        string binResourcesPath = FileHelper.GetPath(Architect.EditorSettings.BinResourcesPath, Game.Profile.FontsPath);

        string jsonFile = name + ".json";
        string pngFile = name + ".png";

        string jsonSourcePackedPath = Path.Join(sourcePackedPath, jsonFile);
        string pngSourcePackedPath = Path.Join(sourcePackedPath, pngFile);

        if (File.Exists(jsonSourcePackedPath) && File.Exists(pngSourcePackedPath))
        {
            // File already exists.
            // TODO: Check for the font size at this point.
            return false;
        }

        FileHelper.CreateDirectoryPathIfNotExists(sourcePackedPath);

        {
            using SKFileWStream stream = new(pngSourcePackedPath);
            using SKPaint skPaint = new();

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

            {
                using SKBitmap bitmap = new(nextPosition, nextPosition, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using SKCanvas canvas = new(bitmap);

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

            FontAsset fontAsset = new(fontIndex, characters, kernings.ToImmutableArray(), fontSize, fontPath, -fontMetrics.Ascent);

            // Save characters to JSON
            FileHelper.SaveSerialized(fontAsset, jsonSourcePackedPath, false);
        }

        // Copy files to binaries path.
        FileHelper.CreateDirectoryPathIfNotExists(binResourcesPath);
        File.Copy(pngSourcePackedPath, Path.Join(binResourcesPath, pngFile));
        File.Copy(jsonSourcePackedPath, Path.Join(binResourcesPath, jsonFile));

        return true;
    }
}