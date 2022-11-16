using Murder.Utilities;

namespace Murder.Editor.Utilities
{
    internal static class EditorImGuiHelpers
    {
        public static int WithDpi(this int value) =>
            Calculator.RoundToInt(value * Architect.EditorSettings.DPI / 100f);
        
        public static System.Numerics.Vector2 WithDpi(this System.Numerics.Vector2 value) =>
            value * Architect.EditorSettings.DPI / 100f;
    }
}
