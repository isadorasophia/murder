using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Input;

public readonly struct InputButtonAxis
{
    public readonly InputButton Up;
    public readonly InputButton Left;
    public readonly InputButton Down;
    public readonly InputButton Right;

    public InputButtonAxis(InputButton up, InputButton left, InputButton down, InputButton right)
    {
        Up = up;
        Left = left;
        Down = down;
        Right = right;
    }
    public InputButtonAxis(Keys up, Keys left, Keys down, Keys right)
    {
        Up = new(up);
        Left = new(left);
        Down = new(down);
        Right = new(right);
    }

    public InputButtonAxis(Buttons up, Buttons left, Buttons down, Buttons right)
    {
        Up = new(up);
        Left = new(left);
        Down = new(down);
        Right = new(right);
    }

    public override string ToString()
    {
        var buttons = new string[] { Up.ToString(), Left.ToString(), Down.ToString(), Right.ToString() };
        return String.Join(", ", buttons);
    }
}
