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
            var nextPosition = 0;

            // Get the font metrics
            SKFontMetrics fontMetrics = new();
            skPaint.GetFontMetrics(out fontMetrics);

            var kernings = new List<Kerning>();

            // There are no defined characters, so we will load the default ASCII characters
            if (chars.IsDefaultOrEmpty)
            {
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
                PackingRectangle[] rectangles = new PackingRectangle[chars.Length];
                const char MissingGlyph = '\u25fb';
                for (var i = 0; i < chars.Length; i++)
                {
                    string charAsString = Char.ToString(chars[i]);
                    // if (!skPaint.Typeface.GetGlyphs(charAsString).Any()) continue;
                    if (!skPaint.Typeface.ContainsGlyph(chars[i])) // Skip if glyph does not exist
                    {
                        charAsString = $"{MissingGlyph}";
                    }

                    skPaint.MeasureText(charAsString, ref bounds);
                    rectangles[i] = new PackingRectangle(0, 0, (uint)bounds.Width + 2, (uint)bounds.Height + 2) { Id = i };
                }
                
                RectanglePacker.Pack(rectangles, out var textureBounds, PackingHints.MostlySquared, 1, 16, 4096, 4096);

                for (var i = 0; i < chars.Length; i++)
                {
                    int charIndex = chars[i];
                    string charAsString = Char.ToString(chars[i]);
                    // if (!skPaint.Typeface.GetGlyphs(charAsString).Any()) continue;
                    if (!skPaint.Typeface.ContainsGlyph(charIndex)) // Skip if glyph does not exist
                    {
                        charAsString = $"{MissingGlyph}";
                    }

                    var adjust = skPaint.MeasureText(charAsString, ref bounds);
                    var advance = skPaint.GetGlyphWidths(charAsString).FirstOrDefault();
                    var offset = skPaint.GetGlyphPositions(charAsString).FirstOrDefault();
                    var fontRect = rectangles[i];
                    
                    var character = new PixelFontCharacter(charIndex,
                        new(fontRect.X, fontRect.Y, fontRect.Width, fontRect.Height),
                        (int)(bounds.Left + offset.X), (int)(bounds.Top + offset.Y), Calculator.CeilToInt(advance));
                    characters[charIndex] = character;
                    maxWidth = Math.Max(maxWidth, (int)bounds.Width);
                }
                
                using SKBitmap bitmap = new((int)textureBounds.Width, (int)textureBounds.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using SKCanvas canvas = new(bitmap);
                
                // Draw each character onto the bitmap
                foreach (var character in characters)
                {
                    var glyph = character.Value.Glyph;
                    
                    if (skPaint.Typeface.ContainsGlyph(character.Key))
                    {
                        canvas.DrawText(((char)character.Key).ToString(),
                            glyph.Left - character.Value.XOffset, glyph.Top - character.Value.YOffset,
                            skPaint);
                    }
                    else
                    {
                        canvas.DrawText(MissingGlyph.ToString(),
                            glyph.Left - character.Value.XOffset, glyph.Top - character.Value.YOffset,
                            skPaint);
                    }
                }

                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }

            EditorTextureServices.ConvertPngStreamToQuoiGz(stream, imageSourcePackedPath);

            // ProcessFinished:
            FontAsset fontAsset = new(
                index:fontIndex, 
                characters: characters, [..kernings], 
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