using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Services;
using Murder.Serialization;
using Murder.Services;
using Murder.Utilities;
using SkiaSharp;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Editor.Data.Graphics;

internal class FontImporter
{
    public static string SourcePackedPath => FileHelper.GetPath(Architect.EditorSettings.SourcePackedPath, Game.Profile.FontsPath);

    public static bool GenerateFontJsonAndPng(int fontIndex, string fontPath, int fontSize, Point fontOffset, Point padding, string name, ImmutableArray<char> chars)
    {
        string sourcePackedPath = SourcePackedPath;
        string binResourcesPath = FileHelper.GetPath(Architect.EditorSettings.BinResourcesPath, Game.Profile.FontsPath);

        string jsonFile = name + ".json";
        string imageFile = name + TextureServices.QOI_GZ_EXTENSION;

        string jsonSourcePackedPath = Path.Join(sourcePackedPath, jsonFile);
        string imageSourcePackedPath = Path.Join(sourcePackedPath, imageFile);

        FileManager.CreateDirectoryPathIfNotExists(sourcePackedPath);

        {
            using MemoryStream stream = new();
            using SKPaint skPaint = new();

            skPaint.IsAntialias = false;
            skPaint.TextSize = fontSize;
            skPaint.Typeface = SKTypeface.FromFile(fontPath);
            skPaint.Color = new SKColor(255, 255, 255);
            skPaint.TextAlign = SKTextAlign.Left;

            var bounds = new SKRect();
            var characters = new Dictionary<int, PixelFontCharacter>();
            var maxWidth = 0;

            // Get the font metrics
            SKFontMetrics fontMetrics = new();
            skPaint.GetFontMetrics(out fontMetrics);

            var kernings = new List<Kerning>();

            // There are no defined characters, so we will load the default ASCII characters
            if (chars.IsDefaultOrEmpty)
            {
                int nextPosition = 0;

                // Measure each character and store in dictionary
                for (int i = 32; i < 255; i++)
                {
                    if (i == 127) continue; // Skip DEL character

                    string charAsString = char.ConvertFromUtf32(i);
                    if (!skPaint.Typeface.GetGlyphs(charAsString).Any()) continue; // Skip if glyph does not exist

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
                for (int i = 32; i < 255; i++)
                {
                    if (i == 127) continue;

                    for (int j = 32; j < 255; j++)
                    {
                        if (i == 127) continue;

                        ushort[] pair = new ushort[] { (ushort)i, (ushort)j };
                        var adjustments = skPaint.Typeface.GetKerningPairAdjustments(pair);
                        if (adjustments.Length > 0)
                        {
                            kernings.Add(new Kerning { First = i, Second = j, Amount = adjustments[0] });
                        }
                    }
                }

                using SKBitmap bitmap = new(nextPosition, nextPosition, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using SKCanvas canvas = new(bitmap);

                // Draw each character onto the bitmap
                foreach (var character in characters)
                {
                    var glyph = character.Value.Glyph;

                    canvas.DrawText(((char)character.Key).ToString(),
                        glyph.Left - character.Value.XOffset, glyph.Top - character.Value.YOffset,
                        skPaint);
                }

                // Save bitmap to PNG
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
            else
            {
                int widthBeforeNextLine = 1080;

                int length = 16;

                int nextPositionX = 0;
                int nextPositionY = 0;

                int maxYOnRow = 0;

                const char MissingGlyph = '\u25fb';
                for (int i = 0; i < chars.Length; i++)
                {
                    int charIndex = chars[i];

                    string charAsString = char.ConvertFromUtf32(chars[i]);
                    if (!skPaint.Typeface.ContainsGlyph(charIndex)) // Skip if glyph does not exist
                    {
                        charAsString = $"{MissingGlyph}";
                    }

                    float adjust = skPaint.MeasureText(charAsString, ref bounds);
                    Point size = new((uint)bounds.Width, (uint)bounds.Height);

                    float advance = skPaint.GetGlyphWidths(charAsString).FirstOrDefault();
                    SKPoint offset = skPaint.GetGlyphPositions(charAsString).FirstOrDefault();

                    if (nextPositionX + length >= widthBeforeNextLine)
                    {
                        nextPositionX = 0;
                        nextPositionY += length;

                        maxYOnRow = 0;
                    }

                    PixelFontCharacter character = new(i,
                        new Rectangle(nextPositionX, nextPositionY, size.X, size.Y),
                        (int)(bounds.Left), 
                        (int)(bounds.Top), 
                        Calculator.CeilToInt(advance));

                    characters[charIndex] = character;

                    nextPositionX += length;
                }

                using SKBitmap bitmap = new(widthBeforeNextLine, nextPositionY + maxYOnRow, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using SKCanvas canvas = new(bitmap);
                
                // Draw each character onto the bitmap
                foreach ((int index, PixelFontCharacter character) in characters)
                {
                    Rectangle glyph = character.Glyph;

                    string charAsString = char.ConvertFromUtf32(index);
                    if (!skPaint.Typeface.ContainsGlyph(index)) // Skip if glyph does not exist
                    {
                        charAsString = $"{MissingGlyph}";
                    }

                    canvas.DrawText(charAsString, glyph.Left - character.XOffset, glyph.Top - character.YOffset, skPaint);
                }

                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            // EditorTextureServices.SaveAsPng(stream, Path.Join(sourcePackedPath, $"{name}.png"));
            EditorTextureServices.ConvertPngStreamToQuoiGz(stream, imageSourcePackedPath);

            // ProcessFinished:
            FontAsset fontAsset = new(
                index: fontIndex, 
                characters: characters, 
                [..kernings], 
                lineHeight: (int)fontMetrics.CapHeight - 1 + padding.Y,
                texturePath: fontPath, 
                baseline: -fontMetrics.Ascent - fontMetrics.Descent,
                offset: fontOffset);

            // Save characters to JSON
            Game.Data.FileManager.SaveSerialized<GameAsset>(fontAsset, jsonSourcePackedPath);
        }

        // Copy files to binaries path.
        FileManager.CreateDirectoryPathIfNotExists(binResourcesPath);
        File.Copy(imageSourcePackedPath, Path.Join(binResourcesPath, imageFile), true);
        File.Copy(jsonSourcePackedPath, Path.Join(binResourcesPath, jsonFile), true);

        return true;
    }
}