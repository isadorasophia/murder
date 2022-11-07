using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using Murder.Core.Geometry;
using Murder.Utilities;
using Murder.Data;
using Murder.Services;

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

        public PixelFontCharacter(int character, AtlasTexture _, XmlElement xml)
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
        public List<AtlasTexture> Textures = new();
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

        public void DrawCharacter(char character, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color)
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
                    c.Glyph,
                    color,
                    0,
                    RenderServices.BLEND_NORMAL
                    );
            }
        }

        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, float scale, int visibleCharacters, Color color, Color? strokeColor, Color? shadowColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var offset = Vector2.Zero;
            var lineWidth = justify.X != 0 ? WidthToNextLine(text, 0) * scale : 0;
            var justified = new Vector2(lineWidth * justify.X, HeightOf(text) * justify.Y);

            for (int i = 0; i < text.Length; i++)
            {
                if (visibleCharacters >= 0 && i >= visibleCharacters)
                    break;

                var character = text[i];

                if (character == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight * scale;
                    if (justify.X != 0)
                        justified.X = WidthToNextLine(text, i + 1) * justify.X;
                    continue;
                }

                if (Characters.TryGetValue(character, out var c))
                {
                    Point pos = (position + (offset + new Vector2(c.XOffset, c.YOffset) * scale - justified)).Floor();
                    var rect = new Rectangle(pos, c.Glyph.Size * scale);
                    var texture = Textures[c.Page];
                    //// draw stroke
                    if (strokeColor.HasValue)
                    {
                        if (shadowColor.HasValue)
                        {
                            texture.Draw(spriteBatch, pos + new Point(-1, 2), c.Glyph, shadowColor.Value, 0, RenderServices.BLEND_NORMAL);
                            texture.Draw(spriteBatch, pos + new Point(0, 2), c.Glyph, shadowColor.Value, 0, RenderServices.BLEND_NORMAL);
                            texture.Draw(spriteBatch, pos + new Point(1, 2), c.Glyph, shadowColor.Value, 0, RenderServices.BLEND_NORMAL);
                        }

                        texture.Draw(spriteBatch, pos + new Point(-1, -1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, -1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, -1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(-1, 0), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 0), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(-1, 1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(0, 1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                        texture.Draw(spriteBatch, pos + new Point(1, 1), c.Glyph, strokeColor.Value, 0, RenderServices.BLEND_NORMAL);
                    }
                    else
                    if (shadowColor.HasValue)
                        texture.Draw(spriteBatch, pos + new Point(0, 1), c.Glyph, shadowColor.Value, 0, RenderServices.BLEND_NORMAL);

                    // draw normal character
                    texture.Draw(spriteBatch, pos, c.Glyph, color, 0, RenderServices.BLEND_NORMAL);

                    offset.X += c.XAdvance * scale;

                    int kerning;
                    if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out kerning))
                        offset.X += kerning * scale;
                }
            }

        }

        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Color color)
        {
            Draw(text, spriteBatch, position, Vector2.Zero, 1f, text.Length, color, null, null);
        }

        public void Draw(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color)
        {
            Draw(text, spriteBatch, position, justify, 1f, text.Length, color, null, null);
        }

        public void DrawOutline(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, Color strokeColor)
        {
            Draw(text, spriteBatch, position, justify, 1f, text.Length, color, null, strokeColor);
        }

        public void DrawEdgeOutline(string text, Batch2D spriteBatch, Vector2 position, Vector2 justify, Color color, Color edgeColor, Color strokeColor)
        {
            Draw(text, spriteBatch, position, justify, 1f, text.Length, color, edgeColor, strokeColor);
        }
    }

    public class PixelFont
    {
        public string Face;
        public List<PixelFontSize> Sizes = new List<PixelFontSize>();

        public PixelFont(string face) { Face = face; }

        public PixelFontSize AddFontSize(XmlElement data, AtlasId atlasId, bool outline = false)
        {
            // check if size already exists
            var size = data["info"]!.AttrFloat("size");
            foreach (var fs in Sizes)
                if (fs.Size == size)
                    return fs;

            // get texture
            var textures = new List<AtlasTexture>();
            XmlElement? pages = data["pages"];
            if (pages is null)
            {
                throw new InvalidOperationException("No pages element found?");
            }

            foreach (XmlElement page in pages)
            {
                var file = page.Attr("file");
                textures.Add(Game.Data.FetchAtlas(atlasId).Get($"fonts/{Path.GetFileNameWithoutExtension(file)}"));
            }

            // create font size
            var fontSize = new PixelFontSize()
            {
                Textures = textures,
                Characters = new Dictionary<int, PixelFontCharacter>(),
                LineHeight = data["common"]!.AttrInt("lineHeight"),
                Size = size,
                Outline = outline
            };

            // get characters
            foreach (XmlElement character in data["chars"]!)
            {
                int id = character.AttrInt("id");
                int page = character.AttrInt("page", 0);
                fontSize.Characters.Add(id, new PixelFontCharacter(id, textures[page], character));
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
            Sizes.Add(fontSize);
            Sizes.Sort((a, b) => { return Math.Sign(a.Size - b.Size); });

            return fontSize;
        }

        public float GetLineWidth(float size, string text)
        {
            var font = Get(size);
            var width = font.WidthToNextLine(text,0);
            return width * (size/font.Size);
        }

        public PixelFontSize Get(float size)
        {
            for (int i = 0, j = Sizes.Count - 1; i < j; i++)
                if (Sizes[i].Size >= size - 1)
                    return Sizes[i];
            return Sizes[Sizes.Count - 1];
        }


        public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Vector2 justify, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            var fontSize = Get(baseSize);
            var scale = baseSize / fontSize.Size;
            fontSize.Draw(text, spriteBatch, position, justify, scale, text.Length, color, strokeColor, shadowColor);
        }

        public void Draw(float baseSize, Batch2D spriteBatch, string text, int visibleCharacters, Vector2 position, Vector2 justify, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            var fontSize = Get(baseSize);
            var scale = baseSize / fontSize.Size;
            fontSize.Draw(text, spriteBatch, position, justify, scale, visibleCharacters, color, strokeColor, shadowColor);
        }

        public void Draw(float baseSize, Batch2D spriteBatch, string text, Vector2 position, Color color, Color? strokeColor = null, Color? shadowColor = null)
        {
            var fontSize = Get(baseSize);
            var scale = baseSize / fontSize.Size;
            fontSize.Draw(text, spriteBatch, position, Vector2.Zero, scale, text.Length, color, strokeColor, shadowColor);
        }

    }
}
