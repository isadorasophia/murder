namespace Murder.Utilities
{
    public static class InputHelpers
    {
        public static void Clamp(int length)
        {
            Game.Input.ClampText(length);
        }

        public static bool FitToWidth(ref ReadOnlySpan<char> text, int width)
        {
            bool changed = false;

            while (Game.Data.PixelFont.GetLineWidth(text) > width)
            {
                text = text[..^1];

                changed = true;
            }

            return changed;
        }
    }
}
