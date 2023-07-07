namespace Murder.Services;

public enum MurderFonts
{
    PixelFont = 100,
    LargeFont = 101
}

public static class MurderFontServices
{
    public static float GetLineWidth(int font, ReadOnlySpan<char> text)
    {
        var f = Game.Data.GetFont(font);
        return f.GetLineWidth(text);
    }
    public static float GetLineWidth(this MurderFonts font, ReadOnlySpan<char> text)
    {
        var f = Game.Data.GetFont((int)font);
        return f.GetLineWidth(text);
    }
}