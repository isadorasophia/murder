using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

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
    public List<MurderTexture> Textures = new();
    public Dictionary<int, PixelFontCharacter> Characters = new();
    public int LineHeight;
    public float BaseLine;

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
                size.Y += LineHeight + 1;
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
            i--;
            if (i > 0 && text.Length > i && (text[i] == ' ' || text[i] == '\n'))
            {
                PixelFontCharacter? c = null;
                if (Characters.TryGetValue(text[i], out c))
                {
                    currentLineWidth -= c.XAdvance;
                    int kerning;
                    if (i < j - 1 && c.Kerning.TryGetValue(text[i + 1], out kerning))
                        currentLineWidth -= kerning;

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

    private record struct TextCacheData(string Text, int Width) { }

    public struct TextCacheDataValue
    {
        public string Text = string.Empty;

        public Dictionary<int, Color?>? Colors = null;
        public HashSet<int>? NonSkippableLineEnding = null;

        public TextCacheDataValue() { }

        public TextCacheDataValue(string text) => Text = text;

        public bool Empty => string.IsNullOrEmpty(Text);
    }

    // [Perf] Cache the last strings parsed.
    private readonly CacheDictionary<TextCacheData, TextCacheDataValue> _cache = new(32);

    /// <summary>
    /// Draw a text with pixel font. If <paramref name="maxWidth"/> is specified, this will automatically wrap the text.
    /// </summary>
    public Point Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale, int visibleCharacters,
        float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth = -1, bool debugBox = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Point.Zero;
        }

        // TODO: Make this an actual api out of this...? So we cache...?

        TextCacheData data = new(text, maxWidth);
        if (!_cache.TryGetValue(data, out TextCacheDataValue parsedText))
        {
            StringBuilder result = new();

            // Replace single newline with space
            text = Regex.Replace(text, "(?<!\n)\n(?!\n)", " ");

            // Replace two consecutive newlines with a single one
            text = Regex.Replace(text, "\n\n", "\n");

            // Replace two or more spaces with a single one
            text = Regex.Replace(text, " {2,}", " ");

            int lastIndex = 0;
            ReadOnlySpan<char> rawText = text;

            MatchCollection matches = Regex.Matches(text, "<c=([^>]+)>([^<]+)</c>");
            if (matches.Count > 0)
            {
                // Map the color indices according to the index in the string.
                // If the color is null, reset to the default color.
                parsedText.Colors = new();

                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    result.Append(rawText.Slice(lastIndex, match.Index - lastIndex));

                    Color colorForText = Color.FromName(match.Groups[1].Value);
                    string currentText = match.Groups[2].Value;

                    // Map the start of this current text as the color switch.
                    parsedText.Colors[result.Length] = colorForText;

                    result.Append(currentText);

                    parsedText.Colors[result.Length] = default;

                    lastIndex = match.Index + match.Length;
                }

                // Look, I also don't think this is correct. But I am still procrastinating doing the right solution for this.
                // So I'll just make sure existing \n do not mess up the color calculation since we are skipping \n
                // when calculating the right color index.
                parsedText.NonSkippableLineEnding = new();
                for (int i = 0; i < rawText.Length; ++i)
                {
                    if (rawText[i] == '\n')
                    {
                        parsedText.NonSkippableLineEnding.Add(i);
                    }
                }
            }

            if (lastIndex < rawText.Length)
            {
                result.Append(rawText.Slice(lastIndex));
            }

            parsedText.Text = result.ToString();
            if (maxWidth > 0)
            {
                string wrappedText = WrapString(parsedText.Text, maxWidth, scale.X, ref visibleCharacters);
                parsedText.Text = wrappedText.ToString();
            }

            _cache[data] = parsedText;
        }
        else if (visibleCharacters >= text.Length)
        {
            // Add the additional lines to the visible characters.
            visibleCharacters += parsedText.Text.Length - text.Length;
        }

        return DrawImpl(parsedText, spriteBatch, position, justify, scale, sort, color, strokeColor, shadowColor, debugBox, visibleCharacters);
    }

    public Point DrawSimple(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale,
        float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox = false) =>
        DrawImpl(new(text), spriteBatch, position, justify, scale, sort, color, strokeColor, shadowColor, debugBox,
            visibleCharacters: text.Length);

    private Point DrawImpl(TextCacheDataValue textData, Batch2D spriteBatch, Vector2 position, Vector2 justify, Vector2 scale,
        float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox, int visibleCharacters)
    {
        if (textData.Empty)
        {
            return Point.Zero;
        }

        string text = textData.Text;

        position = position.Floor();

        Vector2 offset = Vector2.Zero;
        Vector2 justified = new(WidthToNextLine(text, 0) * justify.X, HeightOf(text) * justify.Y);

        Color currentColor = color;

        // Index color, which will track the characters without a new line.
        int indexColor = 0;
        int lineCount = 1;

        // Keep track of the (actual) width of the line.
        float currentWidth = 0;
        float maxLineWidth = 0;

        // Finally, draw each character
        for (int i = 0; i < text.Length; i++, indexColor++)
        {
            maxLineWidth = MathF.Max(maxLineWidth, currentWidth);

            char character = text[i];

            bool isPostAddedLineEnding = textData.NonSkippableLineEnding is null || !textData.NonSkippableLineEnding.Contains(i);
            if (character == '\n')
            {
                currentWidth = 0;

                lineCount++;
                offset.X = 0;
                offset.Y += LineHeight * scale.Y + 1;

                if (justify.X != 0)
                {
                    justified.X = WidthToNextLine(text, i + 1) * justify.X;
                }

                if (isPostAddedLineEnding)
                {
                    indexColor--;
                }

                continue;
            }

            if (visibleCharacters >= 0 && i > visibleCharacters)
            {
                break;
            }

            if (Characters.TryGetValue(character, out var c))
            {
                Point pos = (position + (offset + new Vector2(c.XOffset, c.YOffset + BaseLine + 1) * scale - justified)).Floor();

                var texture = Textures[c.Page];

                // ==== Draw stroke ====
                if (strokeColor.HasValue)
                {
                    if (shadowColor.HasValue)
                    {
                        texture.Draw(spriteBatch, pos + new Point(-1, 2) * scale, scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, 2) * scale, scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 2) * scale, scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                    }

                    texture.Draw(spriteBatch, pos + new Point(-1, -1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(0, -1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, -1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(-1, 0) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, 0) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(-1, 1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(0, 1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    texture.Draw(spriteBatch, pos + new Point(1, 1) * scale, scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                }
                else if (shadowColor.HasValue)
                {
                    // Use 0.001f as the sort so draw the shadow under the font.
                    texture.Draw(spriteBatch, pos + new Point(0, 1), Vector2.One * scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                }

                if (textData.Colors is not null && textData.Colors.TryGetValue(indexColor, out Color? targetColorForText))
                {
                    currentColor = targetColorForText * color.A ?? color;
                }

                // draw normal character
                texture.Draw(spriteBatch, pos, Vector2.One * scale, c.Glyph, currentColor, ImageFlip.None, sort, RenderServices.BLEND_NORMAL);

                offset.X += c.XAdvance * scale.X;
                currentWidth += c.XAdvance * scale.X;

                if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out int kerning))
                {
                    offset.X += kerning * scale.X;
                }
            }
        }
        maxLineWidth = MathF.Max(maxLineWidth, currentWidth);

        Point size = new Point(maxLineWidth, (LineHeight + 1) * lineCount);

        if (debugBox)
        {
            RenderServices.DrawRectangleOutline(spriteBatch, new Rectangle(position - size * justify, size), Color.White, 1, 0);
        }

        return size;
    }

    private string WrapString(ReadOnlySpan<char> text, int maxWidth, float scale, ref int visibleCharacters)
    {
        Vector2 offset = Vector2.Zero;

        StringBuilder wrappedText = new StringBuilder();
        int cursor = 0;
        while (cursor < text.Length)
        {
            bool alreadyHasLineBreak = false;

            int nextSpaceIndex = text.Slice(cursor).IndexOf(' ');
            int nextLineBreak = text.Slice(cursor).IndexOf('\n');

            if (nextLineBreak >= 0 && nextLineBreak < nextSpaceIndex)
            {
                alreadyHasLineBreak = true;
                nextSpaceIndex = nextLineBreak + cursor;
            }
            else if (nextSpaceIndex >= 0)
            {
                nextSpaceIndex = nextSpaceIndex + cursor;
            }

            if (nextSpaceIndex == -1)
                nextSpaceIndex = text.Length - 1;

            ReadOnlySpan<char> word = text.Slice(cursor, nextSpaceIndex - cursor + 1);
            float wordWidth = WidthToNextLine(word, 0, false) * scale;
            bool overflow = offset.X + wordWidth > maxWidth;

            if (overflow)
            {
                // Has overflow, word is going down.
                wrappedText.Append('\n');
                visibleCharacters += 1;
                offset.X = wordWidth;
            }
            else
            {
                // Didn't break line
                offset.X += wordWidth;
            }

            if (alreadyHasLineBreak)
            {
                // Snap to zero.
                offset.X = 0;
            }

            // Make sure we also take the new line into consideration.
            if (visibleCharacters > cursor)
            {
                visibleCharacters += word.Length;
            }

            wrappedText.Append(word);

            cursor = nextSpaceIndex + 1;
        }

        return wrappedText.ToString();
    }

    public void Draw(string text, Batch2D spriteBatch, Vector2 position, Color color, float sort = 0.1f)
    {
        Draw(text, spriteBatch, position, Vector2.Zero, Vector2.One, text.Length, sort, color, null, null);
    }

    public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort = 0.1f)
    {
        Draw(text, spriteBatch, position, justify, Vector2.One, text.Length, sort, color, null, null);
    }
}

public class PixelFont
{
    private readonly PixelFontSize? _pixelFontSize;
    public int Index;

    public int LineHeight => _pixelFontSize?.LineHeight ?? 0;

    public PixelFont(FontAsset asset)
    {
        // get texture
        var textures = new List<MurderTexture>();
        textures.Add(new MurderTexture($"fonts/{Path.GetFileNameWithoutExtension(asset.TexturePath)}"));

        Index = asset.Index;

        // create font size
        var fontSize = new PixelFontSize()
        {
            Textures = textures,
            Characters = new Dictionary<int, PixelFontCharacter>(),
            LineHeight = asset.LineHeight,
            BaseLine = asset.Baseline
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

        //Sizes.Add(fontSize);
        //Sizes.Sort((a, b) => { return Math.Sign(a.Size - b.Size); });
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

    //public PixelFontSize Get(float size)
    //{
    //    for (int i = 0, j = Sizes.Count - 1; i < j; i++)
    //        if (Sizes[i].Size >= size - 1)
    //            return Sizes[i];
    //    return Sizes[Sizes.Count - 1];
    //}

    public Point Draw(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth = -1, int visibleCharacters = -1, bool debugBox = false)
    {
        if (_pixelFontSize == null)
        {
            return Point.Zero;
        }

        return _pixelFontSize.Draw(text, spriteBatch, position, alignment, scale, visibleCharacters >= 0 ? visibleCharacters : text.Length, sort, color, strokeColor, shadowColor, maxWidth, debugBox);
    }

    public Point DrawSimple(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, Vector2 scale, float sort, Color color, Color? strokeColor, Color? shadowColor, bool debugBox = false)
    {
        if (_pixelFontSize == null)
        {
            return Point.Zero;
        }

        return _pixelFontSize.DrawSimple(text, spriteBatch, position, alignment, scale, sort, color, strokeColor, shadowColor, debugBox);
    }

    public static string Escape(string text) => Regex.Replace(text, "<c=([^>]+)>|</c>", "");
}