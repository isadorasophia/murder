using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public MenuInfo Clamp(int max)
        {
            return new MenuInfo(Math.Clamp(Selection, 0, max), LastMoved, LastPressed, Canceled)
            {
                Disabled = Disabled,
                Overflow = Overflow
            };
        }

        public MenuInfo Enabled(bool disabled)
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