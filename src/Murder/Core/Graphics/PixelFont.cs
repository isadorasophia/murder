using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Murder.Core.Graphics;

public class PixelFontCharacter
{
    public int Character;
    public Rectangle Glyph = Rectangle.Empty;
    public int XOffset;
    public int YOffset;
    public int XAdvance;
    public int Page;

    public ImmutableDictionary<int, int> Kerning = ImmutableDictionary<int, int>.Empty;

    public PixelFontCharacter() { }

    public PixelFontCharacter(int character, Rectangle rectangle, int xOffset, int yOffset, int xAdvance)
    {
        Character = character;
        Glyph = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        XOffset = xOffset;
        YOffset = yOffset;
        XAdvance = xAdvance;
    }
}

public class PixelFontSize
{
    public MurderTexture[] Textures = Array.Empty<MurderTexture>();
    public Dictionary<int, PixelFontCharacter> Characters = new();
    public int LineHeight;
    public float BaseLine;
    public Point Offset;

    /// <summary>
    /// Index of the font that this belongs to.
    /// </summary>
    public int Index { get; init; }

    private readonly StringBuilder _temp = new StringBuilder();

    public string AutoNewline(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        _temp.Clear();

        var words = Regex.Split(text, @"(\s)");
        var lineWidth = 0f;

        foreach (var word in words)
        {
            var wordWidth = Measure(word).X;
            if (wordWidth + lineWidth > width)
            {
                _temp.Append('\n');
                lineWidth = 0;

                if (word.Equals(" "))
                    continue;
            }

            // this word is longer than the max-width, split where ever we can
            if (wordWidth > width)
            {
                int i = 1, start = 0;
                for (; i < word.Length; i++)
                    if (i - start > 1 && Measure(word.Substring(start, i - start - 1)).X > width)
                    {
                        _temp.Append(word.Substring(start, i - start - 1));
                        _temp.Append('\n');
                        start = i - 1;
                    }


                var remaining = word.Substring(start, word.Length - start);
                _temp.Append(remaining);
                lineWidth += Measure(remaining).X;
            }
            // normal word, add it
            else
            {
                lineWidth += wordWidth;
                _temp.Append(word);
            }
        }

        return _temp.ToString();
    }

    public Vector2 Measure(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Vector2.Zero;

        var size = new Vector2(0, LineHeight);
        var currentLineWidth = 0f;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                size.Y += LineHeight;
                if (currentLineWidth > size.X)
                    size.X = currentLineWidth;
                currentLineWidth = 0f;
            }
            else
            {
                PixelFontCharacter? c = null;
                if (Characters.TryGetValue(text[i], out c))
                {
                    currentLineWidth += c.XAdvance;

                    int kerning;
                    if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out kerning))
                        currentLineWidth += kerning;
                }
            }
        }

        if (currentLineWidth > size.X)
            size.X = currentLineWidth;

        return size;
    }

    public float WidthToNextLine(ReadOnlySpan<char> text, int start, bool trimWhitespace = true)
    {
        if (text.IsEmpty)
        {
            return 0;
        }

        var currentLineWidth = 0f;

        int i, j;
        for (i = start, j = text.Length; i < j; i++)
        {
            if (text[i] == '\n')
                break;

            PixelFontCharacter? c = null;
            if (Characters.TryGetValue(text[i], out c))
            {
                currentLineWidth += c.XAdvance;
                int kerning;
                if (i < j - 1 && c.Kerning.TryGetValue(text[i + 1], out kerning))
                    currentLineWidth += kerning;
            }
        }

        // Trim ending whitepace
        if (trimWhitespace)
        {
            while (i > 0)
            {
                i--;

                char current = text[i];
                if (i > 0 && text.Length > i && (current == ' ' || current == '\n'))
                {
                    PixelFontCharacter? c = null;
                    if (Characters.TryGetValue(text[i], out c))
                    {
                        currentLineWidth -= c.XAdvance;
                        int kerning;
                        if (i < j - 1 && c.Kerning.TryGetValue(text[i + 1], out kerning))
                            currentLineWidth -= kerning;
                    }
                    else
                    {
                        GameLogger.Warning($"Character '{current}' not found in font.");
                    }
                }
                else
                {
                    break;
                }
            }
        }
        return currentLineWidth;
    }

    public float HeightOf(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        int lines = 1;
        if (text.IndexOf('\n') >= 0)
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\n')
                    lines++;
        return lines * LineHeight;
    }

    /// <summary>
    /// Draw a text with pixel font. If <paramref name="maxWidth"/> is specified, this will automatically wrap the text.
    /// </summary>
    public Point Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 origin, Vector2 scale, int visibleCharacters,
        float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth = -1, bool debugBox = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Point.Zero;
        }

        RuntimeTextData data = TextDataServices.GetOrCreateText(this, text, new TextSettings() { MaxWidth = maxWidth, Scale = scale });
        return DrawImpl(data, spriteBatch, position, origin, scale, sort, color, strokeColor, shadowColor, debugBox, visibleCharacters);
    }

    public Point Draw(RuntimeTextData data, Batch2D spriteBatch, Vector2 position, Vector2 origin, Vector2 scale, int visibleCharacters,
        float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox = false) =>
        DrawImpl(data, spriteBatch, position, origin, scale, sort, color, strokeColor, shadowColor, debugBox, visibleCharacters);

    public Point DrawSimple(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale,
        float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox = false) =>
        DrawImpl(new(text), spriteBatch, position, justify, scale, sort, color, strokeColor, shadowColor, debugBox,
            visibleCharacters: text.Length);

    private Point DrawImpl(RuntimeTextData textData, Batch2D spriteBatch, Vector2 position, Vector2 origin, Vector2 scale,
        float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox, int visibleCharacters)
    {
        if (textData.Empty)
        {
            return Point.Zero;
        }

        if (visibleCharacters == 0)
        {
            return Point.Zero;
        }

        string text = textData.Text;

        position = position.Round();

        Vector2 offset = Offset;
        Vector2 justified = new(WidthToNextLine(text, 0, true) * origin.X * scale.X, HeightOf(text) * origin.Y * scale.Y);

        Color currentColor = color;

        // Index color, which will track the characters without a new line.
        int letterIndex = 0;
        int lineCount = 1;

        // Keep track of the (actual) width of the line.
        float currentWidth = 0;
        float maxLineWidth = 0;

        //Glitch tracking
        float glitchAmount = 0;

        // Finally, draw each character
        for (int i = 0; i < text.Length; i++, letterIndex++)
        {
            maxLineWidth = MathF.Max(maxLineWidth, currentWidth);

            char character = text[i];

            // check for letter properties before doing anything else. 
            // even if this is an invalid character, we still need to keep track of its properties
            RuntimeLetterProperties? letter = textData.TryGetLetterProperty(letterIndex);
            if (letter is not null)
            {
                if (letter.Value.Glitch != 0)
                {
                    glitchAmount = letter.Value.Glitch;
                }
                if (letter.Value.Properties.HasFlag(RuntimeLetterPropertiesFlag.ResetGlitch))
                {
                    glitchAmount = 0;
                }

                if (letter.Value.Color is Color newColor)
                {
                    currentColor = newColor * color.A;
                }
                else if (letter.Value.Properties.HasFlag(RuntimeLetterPropertiesFlag.ResetColor))
                {
                    currentColor = color;
                }
            }

            // i don't think we need this anymore?
            //bool isPostAddedLineEnding = letter?.Properties is not RuntimeLetterPropertiesFlag properties ||
            //    !properties.HasFlag(RuntimeLetterPropertiesFlag.DoNotSkipLineEnding);

            if (character == '\n')
            {
                currentWidth = 0;

                lineCount++;
                offset.X = Offset.X;
                offset.Y += LineHeight * scale.Y;

                if (origin.X != 0)
                {
                    justified.X = Calculator.FloorToInt(WidthToNextLine(text, i + 1) * origin.X); // This is suspicious, shound't this be round?
                }

                continue;
            }

            if (visibleCharacters >= 0 && letterIndex > visibleCharacters)
            {
                break;
            }

            if (Characters.TryGetValue(character, out PixelFontCharacter? c))
            {
                float waveOffset = 0f;
                Vector2 shake = Vector2.Zero;
                if (letter is not null)
                {
                    if (letter.Value.Properties.HasFlag(RuntimeLetterPropertiesFlag.Wave))
                    {
                        waveOffset = MathF.Sin(Game.NowUnscaled * 4f + i);
                    }
                    if (letter.Value.Properties.HasFlag(RuntimeLetterPropertiesFlag.Fear))
                    {
                        float amplitude = .9f;
                        float smoothFactor = Math.Abs(MathF.Sin(Game.NowUnscaled * 16f + i * 2));
                        smoothFactor *= smoothFactor;
                        shake = new Vector2(Game.Random.NextFloat(-smoothFactor, smoothFactor) * amplitude, Game.Random.NextFloat(-smoothFactor, smoothFactor) * amplitude);
                    }
                }

                Vector2 effects = new Vector2(shake.X, shake.Y + waveOffset);

                Point pos = (position + (offset + new Vector2(c.XOffset, c.YOffset + LineHeight - 1) * scale - justified) + effects).Floor();

                var texture = Textures[c.Page];
                Rectangle glyph = c.Glyph;

                if (glitchAmount != 0)
                {
                    float seed = (Game.NowUnscaled) % 64 + i;
                    float glitch = glitchAmount * 0.8f;
                    if (RandomExtensions.SmoothRandom(((int)(seed * 10 + i) % 64) / 64f, 10.5f) > 0.96f * (1 - glitch))
                    {
                        float val = RandomExtensions.SmoothRandom(seed + i * 7.1f, 10) * 5;
                        character = (char)((int)character + val);
                        if (Characters.TryGetValue(character, out var newC))
                        {
                            glyph = newC.Glyph;
                        }
                    }

                    if (RandomExtensions.SmoothRandom(seed * 2, 10.5f) < 0.25f * glitch)
                    {
                        if (Game.Random.TryWithChanceOf(0.72f * glitch))
                        {
                            pos += new Point(Game.Random.Next(-2, 2), Game.Random.Next(-2, 2));
                        }
                        if (Game.Random.TryWithChanceOf(0.515f * glitch + Math.Abs(0.05f * MathF.Sin(seed * 2))))
                        {
                            glyph = new Rectangle(
                                glyph.X + Game.Random.Next(-4, 4),
                                glyph.Y + Game.Random.Next(-4, 4),
                                glyph.Width + Game.Random.Next(-8, 8),
                                glyph.Height + +Game.Random.Next(-8, 8));
                        }
                    }
                    if (RandomExtensions.SmoothRandom(seed + i * 10.1f, 0.5f) < 0.5f * glitch)
                    {
                        pos.Y += (int)(RandomExtensions.SmoothRandom(seed, 30) * 3 - 1.5f);
                    }

                    if (RandomExtensions.SmoothRandom(seed * 2 + i * 20.1f, 0.5f) < 0.5f * glitch)
                    {
                        float val = 2f * MathF.Sin(seed * 2 + i);
                        glyph = new Rectangle(
                            glyph.X + val,
                            glyph.Y + val,
                            glyph.Width + val,
                            glyph.Height + +val);
                    }
                }
                // ==== Draw stroke ====
                if (strokeColor.HasValue)
                {
                    if (shadowColor.HasValue)
                    {
                        texture.Draw(spriteBatch, pos + new Point(-1, 2) * scale, scale, glyph, shadowColor.Value, ImageFlip.None, sort + 0.0002f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, 2) * scale, scale, glyph, shadowColor.Value, ImageFlip.None, sort + 0.0002f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 2) * scale, scale, glyph, shadowColor.Value, ImageFlip.None, sort + 0.0002f, RenderServices.BLEND_NORMAL);
                    }

                    texture.Draw(spriteBatch, pos + new Point(-1, -1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(0, -1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, -1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(-1, 0) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, 0) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(-1, 1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(0, 1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, 1) * scale, scale, glyph, strokeColor.Value, ImageFlip.None, sort + 0.0001f, RenderServices.BLEND_NORMAL);
                }
                else if (shadowColor.HasValue)
                {
                    // Use 0.001f as the sort so draw the shadow under the font.
                    texture.Draw(spriteBatch, pos + new Point(0, 1), Vector2.One * scale, glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                }

                // draw normal character
                texture.Draw(spriteBatch, pos, Vector2.One * scale, glyph, currentColor, ImageFlip.None, sort, RenderServices.BLEND_NORMAL);

                offset.X += c.XAdvance * scale.X;
                currentWidth += c.XAdvance * scale.X;

                if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out int kerning))
                {
                    offset.X += kerning * scale.X;
                }
            }
        }
        maxLineWidth = MathF.Max(maxLineWidth, currentWidth);

        Point size = new Point(maxLineWidth, (int)(LineHeight * lineCount * scale.Y));

        if (debugBox)
        {
            RenderServices.DrawHorizontalLine(spriteBatch, (int)position.X - 4, (int)position.Y, 8, Color.Red, 0);
            RenderServices.DrawVerticalLine(spriteBatch, (int)position.X, (int)position.Y - 4, 8, Color.Red, 0);

            RenderServices.DrawRectangleOutline(spriteBatch, new Rectangle(position - size * origin, size), Color.White, 1, 0.001f);
        }

        return size;
    }

    public string WrapString(ReadOnlySpan<char> text, int maxWidth, float scale)
    {
        Vector2 offset = Vector2.Zero;

        bool lineBreakOnSpace = LocalizationServices.IsTextWrapOnlyOnSpace();

        StringBuilder wrappedText = new StringBuilder();
        int cursor = 0;
        while (cursor < text.Length)
        {
            bool alreadyHasLineBreak = false;

            int nextSeparatorIndex = lineBreakOnSpace ? text[cursor..].IndexOf(' ') : 1;
            int nextLineBreak = text[cursor..].IndexOf('\n');

            if (nextLineBreak >= 0 && nextLineBreak < nextSeparatorIndex)
            {
                alreadyHasLineBreak = true;
                nextSeparatorIndex = nextLineBreak + cursor;
            }
            else if (nextSeparatorIndex >= 0)
            {
                nextSeparatorIndex = nextSeparatorIndex + cursor;
            }

            if (nextSeparatorIndex == -1 || nextSeparatorIndex >= text.Length - 1)
                nextSeparatorIndex = text.Length - 1;

            ReadOnlySpan<char> word = text.Slice(cursor, nextSeparatorIndex - cursor + 1);
            float wordWidth = WidthToNextLine(word, 0, true) * scale;
            bool overflow = offset.X + wordWidth > maxWidth;

            if (overflow)
            {
                // Has overflow, word is going down.
                if (wrappedText.Length != 0)
                {
                    // Trim wrapped text of any whitespace at the end.
                    while (wrappedText.Length > 0 && wrappedText[^1] == ' ')
                    {
                        wrappedText.Length--;
                    }
                    wrappedText.Append('\n');
                }

                offset.X = wordWidth;
            }
            else
            {
                // Didn't break line
                offset.X += WidthToNextLine(word, 0, false); // Add the width of the word plus any whitespace.
            }

            if (alreadyHasLineBreak)
            {
                // Snap to zero.
                offset.X = 0;
            }

            wrappedText.Append(word);

            cursor = nextSeparatorIndex + 1;
        }

        return wrappedText.ToString();
    }
}

public class PixelFont
{
    private readonly PixelFontSize _pixelFontSize;
    public int Index;

    public int LineHeight => _pixelFontSize.LineHeight;

    public PixelFontSize PixelFontSize => _pixelFontSize;

    public PixelFont(FontAsset asset)
    {
        // get textures
        MurderTexture[] textures =
            [new MurderTexture($"fonts/{Path.GetFileNameWithoutExtension(asset.TexturePath)}")];

        Index = asset.Index;

        // create font size
        PixelFontSize fontSize = new()
        {
            Textures = textures,
            Characters = [],
            LineHeight = asset.LineHeight,
            BaseLine = asset.Baseline,
            Offset = asset.Offset,
            Index = asset.Index
        };

        // get characters
        foreach (var character in asset.Characters)
        {
            fontSize.Characters.Add(character.Key, character.Value);
        }

        // get kerning
        foreach (var kerning in asset.Kernings)
        {
            var from = kerning.First;
            var to = kerning.Second;
            var push = kerning.Amount;

            if (fontSize.Characters.TryGetValue(from, out var c))
                c.Kerning.Add(to, push);
        }

        // add font size
        _pixelFontSize = fontSize;
    }

    public float GetLineWidth(ReadOnlySpan<char> text)
    {
        if (_pixelFontSize is null)
        {
            GameLogger.Error("Pixel font size was not initialized.");
            return -1;
        }

        //var font = Get(size);
        float width = _pixelFontSize.WidthToNextLine(text, 0);
        return width;
    }

    public Point Draw(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color, Color? strokeColor,
        Color? shadowColor, int maxWidth = -1, int visibleCharacters = -1, bool debugBox = false)
    {
        if (_pixelFontSize == null || string.IsNullOrEmpty(text))
        {
            return Point.Zero;
        }

        return _pixelFontSize.Draw(text, spriteBatch, position, alignment, scale, visibleCharacters >= 0 ? visibleCharacters : text.Length,
            sort, color, strokeColor, shadowColor, maxWidth, debugBox);
    }

    public Point Draw(Batch2D spriteBatch, RuntimeTextData data, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color,
        Color? strokeColor, Color? shadowColor, int visibleCharacters = -1, bool debugBox = false)
    {
        if (_pixelFontSize == null)
        {
            return Point.Zero;
        }

        return _pixelFontSize.Draw(data, spriteBatch, position, alignment, scale, visibleCharacters >= 0 ? visibleCharacters : data.Length,
            sort, color, strokeColor, shadowColor, debugBox);
    }

    public Point DrawSimple(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort,
        Color color, Color? strokeColor, Color? shadowColor, bool debugBox = false)
    {
        if (_pixelFontSize == null)
        {
            return Point.Zero;
        }

        return _pixelFontSize.DrawSimple(text, spriteBatch, position, alignment, scale, sort, color, strokeColor, shadowColor, debugBox);
    }

    public void Preload()
    {
        foreach (MurderTexture t in _pixelFontSize.Textures)
        {
            t.Preload();
        }
    }

    public static string Escape(string text) => TextDataServices.EscapeRegex().Replace(text, "");
}