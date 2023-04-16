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
        public MenuInfo Enabled(bool disabled)
        {
            return new MenuInfo(Selection, LastMoved, LastPressed, Canceled)
            {
                Disabled = disabled
            };
        }
    }
}