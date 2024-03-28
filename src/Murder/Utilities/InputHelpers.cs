using Murder.Services;

namespace Murder.Utilities
{
    public static class InputHelpers
    {
        public static void Clamp(int length)
        {
            Game.Input.ClampText(length);
        }

        public static int GetAmountOfLines(ReadOnlySpan<char> text, int width)
        {
            float lineWidth = MurderFonts.PixelFont.GetLineWidth(text);
            return Calculator.RoundToInt(lineWidth / width);
        }

        public static bool FitToWidth(ref ReadOnlySpan<char> text, int width)
        {
            bool changed = false;

            while (MurderFonts.PixelFont.GetLineWidth(text) > width)
            {
                text = text[..^1];

                changed = true;
            }

            return changed;
        }

        public static string IntToDPad(int slot)
        {
            switch (slot)
            {
                case 0:
                    return "D-Down";
                case 1:
                return "D-Left";
                case 2:
                    return "D-Up";
                case 3:
                    return "D-Right";
                default:
                    return "D-Down";
            }
        }
    }
}