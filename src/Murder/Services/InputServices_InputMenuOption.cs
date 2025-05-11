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
        public readonly int? Id { get;init; } = 0;
        public readonly InputStyle Style { get; init; } = 0;

        public enum InputStyle
        {
            None = 0,
            Button = 1,
            Axis = 2,
        }

        public InputMenuOption(string text, InputStyle style, int? buttonId)
        {
            Text = text;
            Id = buttonId;
            Style = style;
        }
    }
}
