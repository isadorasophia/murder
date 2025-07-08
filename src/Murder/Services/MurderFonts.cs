using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Services;

[Font]
public enum MurderFonts
{
    PixelFont = 100,
    LargeFont = 101,
    SmallFont = 102
}

public static class MurderFontServices
{

    public static Point MeasureText(int font, string text, bool cultureInvariant = false)
    {
        PixelFont f = Game.Data.GetFont(font, cultureInvariant);

        return new Point(f.GetLineWidth(text), f.PixelFontSize.LineHeight);
    }

    public static float GetLineWidth(this MurderFonts font, ReadOnlySpan<char> text)
    {
        PixelFont f = Game.Data.GetFont((int)font);
        return f.GetLineWidth(text);
    }

    public static float GetLineHeight(RuntimeTextData text)
    {
        PixelFont f = Game.Data.GetFont(text.Font);
        return f.PixelFontSize.HeightOf(text.Text);
    }
    public static float GetLineHeight(int font, string text)
    {
        PixelFont f = Game.Data.GetFont(font);
        return f.PixelFontSize.HeightOf(text);
    }

    public static float GetLineWidth(int font, string text)
    {
        PixelFont f = Game.Data.GetFont(font);
        RuntimeTextData runtimeText = TextDataServices.GetOrCreateText(font, text, new TextSettings());

        return f.GetLineWidth(runtimeText.Text);
    }

    public static float GetLineWidth(this MurderFonts font, string text)
    {
        return GetLineWidth((int)font, text);
    }

    public static int GetFontHeight(this MurderFonts font, bool cultureInvariant = false)
    {
        PixelFont f = Game.Data.GetFont((int)font, cultureInvariant);
        return f.LineHeight;
    }
}