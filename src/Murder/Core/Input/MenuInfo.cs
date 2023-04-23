using Murder.Utilities;

namespace Murder.Core.Input
{
    public record struct MenuInfo(
        int Selection,
        float LastMoved,
        float LastPressed,
        bool Canceled)
    {
        public bool Disabled = false;
        public int Overflow = 0;

        public void Clamp(int max)
        {
            Selection = Math.Max(0, Math.Min(Selection, max));
        }

        public MenuInfo Disable(bool disabled)
        {
            return new MenuInfo(Selection, LastMoved, LastPressed, Canceled)
            {
                Disabled = disabled,
                Overflow = Overflow
            };
        }

        public void SnapRight(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width) + 1) * width-1;
        }
        public void SnapLeft(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width)) * width;
        }
    }
}