using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Data;
using Murder.Services;
using Murder.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using Murder.Core.Dialogs;

namespace Murder.Core.Graphics
{
    public class PixelFontCharacter
    {
        public int Character;
        public Rectangle Glyph;
        public int XOffset;
        public int YOffset;
        public int XAdvance;
        public int Page;

        public Dictionary<int, int> Kerning = new Dictionary<int, int>();

        public PixelFontCharacter(int character, XmlElement xml)
        {
            Character = character;
            Glyph = new Rectangle(xml.AttrInt("x"), xml.AttrInt("y"), xml.AttrInt("width"), xml.AttrInt("height"));
            XOffset = xml.AttrInt("xoffset");
            YOffset = xml.AttrInt("yoffset");
            XAdvance = xml.AttrInt("xadvance");
        }
    }

    public class PixelFontSize
    {
        public List<MurderTexture> Textures = new();
        public Dictionary<int, PixelFontCharacter> Characters = new();
        public int LineHeight;
        public float Size;
        public bool Outline;

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

        public PixelFontCharacter? Get(int id)
        {
            if (Characters.TryGetValue(id, out var val))
                return val;
            return null;
        }

        public Vector2 Measure(char text)
        {
            if (Characters.TryGetValue(text, out var c))
                return new Vector2(c.XAdvance, LineHeight);
            return Vector2.Zero;
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

        public float WidthToNextLine(string text, int start)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var currentLineWidth = 0f;
            
            for (int i = start, j = text.Length; i < j; i++)
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

        public void DrawCharacter(char character, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort)
        {
            if (char.IsWhiteSpace(character))
                return;

            if (Characters.TryGetValue(character, out var c))
            {
                var measure = Measure(character);
                var justified = new Vector2(measure.X * justify.X, measure.Y * justify.Y);
                var pos = position + (new Vector2(c.XOffset, c.YOffset) - justified);

                Textures[c.Page].Draw(
                    spriteBatch,
                    pos.Floor(),
                    Vector2.One,
                    c.Glyph,
                    color,
                    ImageFlip.None,
                    sort,
                    RenderServices.BLEND_NORMAL
                    );
            }
        }
        
        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, float scale, int visibleCharacters, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth = -1)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            StringBuilder result = new();
            ReadOnlySpan<char> rawText = text;

            // TODO: Make this an actual api out of this...? So we cache...?

            // Map the color indices according to the index in the string.
            // If the color is null, reset to the default color.
            Dictionary<int, Color?> _colorIndexes = new();

            MatchCollection matches = Regex.Matches(text, "<c=([^>]+)>([^<]+)</c>");

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                result.Append(rawText.Slice(lastIndex, match.Index - lastIndex));

                Color colorForText = Color.FromName(match.Groups[1].Value);
                string currentText = match.Groups[2].Value;

                // Map the start of this current text as the color switch.
                _colorIndexes[result.Length] = colorForText;

                result.Append(currentText);

                _colorIndexes[result.Length] = default;

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < rawText.Length)
            {
                result.Append(rawText.Slice(lastIndex));
            }
            
            string parsedText = result.ToString();
            
            var offset = Vector2.Zero;
            var lineWidth = justify.X != 0 ? WidthToNextLine(parsedText, 0) * scale : 0;

            if (maxWidth > 0)
            {
                var wrappedText = new StringBuilder();
                for (int i = 0; i < parsedText.Length; i++)
                {
                    var nextSpaceIndex = parsedText.IndexOf(' ', i);
                    if (nextSpaceIndex == -1)
                        nextSpaceIndex = parsedText.Length;

                    string remainingText = parsedText.Substring(i, nextSpaceIndex - i);
                    var nextSpaceWidth = WidthToNextLine(remainingText, 0) * scale;
                    if (offset.X + nextSpaceWidth > maxWidth)
                    {
                        offset.X = 0;
                        wrappedText.Append('\n');
                    }
                    
                    if (Characters.TryGetValue(parsedText[i], out var c))
                    {
                        wrappedText.Append(parsedText[i]);
                        offset.X += c.XAdvance * scale;
                        int kerning;
                        if (i < parsedText.Length - 1 && c.Kerning.TryGetValue(parsedText[i + 1], out kerning))
                            offset.X += kerning * scale;
                    }
                }

                parsedText = wrappedText.ToString();
                offset.X = 0;
            }

            var justified = new Vector2(lineWidth * justify.X, HeightOf(parsedText) * justify.Y);

            Color currentColor = color;

            for (int i = 0; i < parsedText.Length; i++)
            {
                var character = parsedText[i];
                
                if (character == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight * scale + 1;
                    if (justify.X != 0)
                        justified.X = WidthToNextLine(parsedText, i + 1) * justify.X;
                    continue;
                }

                if (visibleCharacters >= 0 && i >= visibleCharacters)
                    break;
                
                //if (character == '\n')
                //{
                //    offset.X = 0;
                //    offset.Y += LineHeight * scale + 1;
                //    if (justify.X != 0)
                //        justified.X = WidthToNextLine(parsedText, i + 1) * justify.X;
                //    continue;
                //}

                if (Characters.TryGetValue(character, out var c))
                {
                    Point pos = (position.Point + (offset + new Vector2(c.XOffset, c.YOffset) * scale - justified)).Floor();
                    Rectangle rect = new Rectangle(pos, c.Glyph.Size);
                    var texture = Textures[c.Page];
                    //// draw stroke
                    if (strokeColor.HasValue)
                    {
                        if (shadowColor.HasValue)
                        {
                            texture.Draw(spriteBatch, pos + new Point(-1, 2), Vector2.One * scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                            texture.Draw(spriteBatch, pos + new Point(0, 2), Vector2.One * scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                            texture.Draw(spriteBatch, pos + new Point(1, 2), Vector2.One * scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        }

                        texture.Draw(spriteBatch, pos + new Point(-1, -1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, -1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, -1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(-1, 0), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 0), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(-1, 1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, 1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 1), Vector2.One * scale, c.Glyph, strokeColor.Value, ImageFlip.None, sort + 0.001f, RenderServices.BLEND_NORMAL);
                    }
                    else if (shadowColor.HasValue)
                    {
                        // Use 0.001f as the sort so draw the shadow under the font.
                        texture.Draw(spriteBatch, pos + new Point(0, 1), Vector2.One * scale, c.Glyph, shadowColor.Value, ImageFlip.None, sort + 0.002f, RenderServices.BLEND_NORMAL);
                    }

                    if (_colorIndexes.ContainsKey(i))
                    {
                        currentColor = _colorIndexes[i] ?? color;
                    }
                    
                    // draw normal character
                    texture.Draw(spriteBatch, pos, Vector2.One * scale, c.Glyph, currentColor, ImageFlip.None, sort, RenderServices.BLEND_NORMAL);

                    offset.X += c.XAdvance * scale;

                    int kerning;
                    if (i < parsedText.Length - 1 && c.Kerning.TryGetValue(parsedText[i + 1], out kerning))
                        offset.X += kerning * scale;
                }
            }

        }

        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Color color, float sort = 0.1f)
        {
            Draw(text, spriteBatch, position, Vector2.Zero, 1f, text.Length, sort, color, null, null);
        }

        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, float sort = 0.1f)
        {
            Draw(text, spriteBatch, position, justify, 1f, text.Length, sort, color, null, null);
        }
    }

    public class PixelFont
    {
        public string Face;
        private PixelFontSize? _pixelFontSize;

        public int LineHeight => _pixelFontSize?.LineHeight ?? 0;
        // Legacy font sizes
        // public List<PixelFontSize> Sizes = new List<PixelFontSize>();

        public PixelFont(string face) { Face = face; }

        public PixelFontSize AddFontSize(XmlElement data, AtlasId atlasId, bool outline = false)
        {
            // check if size already exists
            //var size = data["info"]!.AttrFloat("size");
            //foreach (var fs in Sizes)
            //    if (fs.Size == size)
            //        return fs;

            // get texture
            var textures = new List<MurderTexture>();
            XmlElement? pages = data["pages"];
            if (pages is null)
            {
                throw new InvalidOperationException("No pages element found?");
            }

            foreach (XmlElement page in pages)
            {
                var file = page.Attr("file");
                if (atlasId == AtlasId.None)
                {
                    textures.Add(new MurderTexture($"fonts/{Path.GetFileNameWithoutExtension(file)}"));
                }
                else
                {
                    textures.Add(new MurderTexture(Game.Data.FetchAtlas(atlasId).Get($"fonts/{Path.GetFileNameWithoutExtension(file)}")));
                }
            }

            // create font size
            var fontSize = new PixelFontSize()
            {
                Textures = textures,
                Characters = new Dictionary<int, PixelFontCharacter>(),
                LineHeight = data["common"]!.AttrInt("lineHeight"),
                Outline = outline
            };

            // get characters
            foreach (XmlElement character in data["chars"]!)
            {
                int id = character.AttrInt("id");
                int page = character.AttrInt("page", 0);
                fontSize.Characters.Add(id, new PixelFontCharacter(id, character));
            }

            // get kerning
            if (data["kernings"] != null)
                foreach (XmlElement kerning in data["kernings"]!)
                {
                    var from = kerning.AttrInt("first");
                    var to = kerning.AttrInt("second");
                    var push = kerning.AttrInt("amount");

                    if (fontSize.Characters.TryGetValue(from, out var c))
                        c.Kerning.Add(to, push);
                }

            // add font size
            _pixelFontSize = fontSize;
            
            //Sizes.Add(fontSize);
            //Sizes.Sort((a, b) => { return Math.Sign(a.Size - b.Size); });

            return fontSize;
        }

        public float GetLineWidth(float size, string text)
        {
            if (_pixelFontSize is null)
            {
                GameLogger.Error("Pixel font size was not initialized.");
                return -1;
            }
            
            //var font = Get(size);
            var width = _pixelFontSize.WidthToNextLine(text,0);
            return width * (size/ _pixelFontSize.Size);
        }

        //public PixelFontSize Get(float size)
        //{
        //    for (int i = 0, j = Sizes.Count - 1; i < j; i++)
        //        if (Sizes[i].Size >= size - 1)
        //            return Sizes[i];
        //    return Sizes[Sizes.Count - 1];
        //}


        public void Draw(Batch2D spriteBatch, string text, float scale, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            _pixelFontSize?.Draw(text, spriteBatch, position, alignment, scale, text.Length, sort, color, strokeColor, shadowColor);
        }

        public void Draw(Batch2D spriteBatch, string text, float scale, int visibleCharacters, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            _pixelFontSize?.Draw(text, spriteBatch, position, alignment, scale, visibleCharacters, sort, color, strokeColor, shadowColor);
        }

        public void Draw(Batch2D spriteBatch, string text, float scale, Vector2 position, float sort, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            _pixelFontSize?.Draw(text, spriteBatch, position, Vector2.Zero, scale, text.Length, sort, color, strokeColor, shadowColor);
        }

        public void Draw(Batch2D spriteBatch, string text, Vector2 position, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth, int visibleCharacters = -1)
        {
            _pixelFontSize?.Draw(text, spriteBatch, position, Vector2.Zero, 1, visibleCharacters >= 0 ? visibleCharacters : text.Length, sort, color, strokeColor, shadowColor, maxWidth);
        }

        public void Draw(Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth, int visibleCharacters = -1)
        {
            _pixelFontSize?.Draw(text, spriteBatch, position, alignment, 1, visibleCharacters >= 0 ? visibleCharacters : text.Length, sort, color, strokeColor, shadowColor, maxWidth);
        }

        // Legacy size
        //public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Vector2 justify, Color color, Color? strokeColor = null, Color? shadowColor = null)
        //{
        //    var fontSize = Get(baseSize);
        //    var scale = baseSize / fontSize.Size;
        //    fontSize.Draw(text, spriteBatch, position, justify, scale, text.Length, color, strokeColor, shadowColor);
        //}

        //public void Draw(float baseSize, Batch2D spriteBatch, string text, int visibleCharacters, Vector2 position, Vector2 justify, Color color, Color? strokeColor = null, Color? shadowColor = null)
        //{
        //    var fontSize = Get(baseSize);
        //    var scale = baseSize / fontSize.Size;
        //    fontSize.Draw(text, spriteBatch, position, justify, scale, visibleCharacters, color, strokeColor, shadowColor);
        //}

        //public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Color color, Color? strokeColor = null, Color? shadowColor = null)
        //{
        //    var fontSize = Get(baseSize);
        //    var scale = baseSize / fontSize.Size;
        //    fontSize.Draw(text, spriteBatch, position, Vector2.Zero, scale, text.Length, color, strokeColor, shadowColor);
        //}

    }
}
