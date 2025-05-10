using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Services
{
    public readonly struct InputMenuOption
    {
        public readonly string Text { get; init; } = string.Empty;
        public readonly int? ButtonId { get;init; } = 0;

        public InputMenuOption(string text, int? buttonId = null)
        {
            Text = text;
            ButtonId = buttonId;
        }
    }
}
