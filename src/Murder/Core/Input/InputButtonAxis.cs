using Microsoft.Xna.Framework.Input;
using System.Numerics;

namespace Murder.Core.Input;

public readonly struct InputButtonAxis
{
    public readonly InputButton? Single;
    public readonly InputButton Up;
    public readonly InputButton Left;
    public readonly InputButton Down;
    public readonly InputButton Right;
    public readonly InputSource Source;

    public InputButtonAxis(InputButton up, InputButton left, InputButton down, InputButton right)
    {
        Source = up.Source;
        Up = up;
        Left = left;
        Down = down;
        Right = right;
    }
    public InputButtonAxis(Keys up, Keys left, Keys down, Keys right)
    {
        Source = InputSource.Keyboard;
        Up = new(up);
        Left = new(left);
        Down = new(down);
        Right = new(right);
    }

    public InputButtonAxis(Buttons up, Buttons left, Buttons down, Buttons right)
    {
        Source = InputSource.Gamepad;
        Up = new(up);
        Left = new(left);
        Down = new(down);
        Right = new(right);
    }

    public InputButtonAxis(GamepadAxis axis)
    {
        Source = InputSource.Gamepad;
        Single = new(axis);
    }

    public override string ToString()
    {
        if (Single != null)
            return Single.Value.ToString();
        
        var buttons = new string[] { Up.ToString(), Left.ToString(), Down.ToString(), Right.ToString() };
        return String.Join(", ", buttons);
    }

    public Vector2 Check(InputState state)
    {
        if (Single != null)
            return Single.Value.GetAxis(state.GamePadState);

        int x =  Right.Check(state) ? 1 : 0;
        int y = Down.Check(state) ? 1 : 0;
        x -= Left.Check(state) ? 1 : 0;
        y -= Up.Check(state) ? 1 : 0;

        return new(x, y);
    }
}
