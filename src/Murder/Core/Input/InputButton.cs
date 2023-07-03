
using Microsoft.Xna.Framework.Input;

namespace Murder.Core.Input;

public readonly struct InputButton
{
    public readonly InputSource Source = InputSource.None;
    public readonly Keys? Keyboard = null;
    public readonly Buttons? Gamepad = null;
    public readonly MouseButtons? Mouse = null;

    public InputButton(Keys key)
    {
        Source = InputSource.Keyboard;
        Keyboard = key;
    }

    public InputButton(Buttons button)
    {
        Source = InputSource.Gamepad;
        Gamepad = button;
    }

    public InputButton(MouseButtons button)
    {
        Source = InputSource.Mouse;
        Mouse = button;        
    }

    public bool Check(InputState state)
    {
        switch (Source)
        {
            case InputSource.None:
                return false;
            case InputSource.Keyboard:
                return state.KeyboardState.IsKeyDown(Keyboard!.Value);
            case InputSource.Gamepad:
                return state.GamePadState.IsButtonDown(Gamepad!.Value);
            case InputSource.Mouse:
                switch (Mouse!.Value)
                {
                    case MouseButtons.Left:
                        return state.MouseState.LeftButton == ButtonState.Pressed;
                    case MouseButtons.Middle:
                        return state.MouseState.MiddleButton == ButtonState.Pressed;
                    case MouseButtons.Right:
                        return state.MouseState.RightButton == ButtonState.Pressed;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }

    internal bool IsAvailable(GamePadCapabilities capabilities)
    {
        switch (Source)
        {
            case InputSource.Keyboard:
                return true;
            case InputSource.Mouse:
                return true;
            case InputSource.Gamepad:
                return capabilities.IsConnected;
            case InputSource.None:
            default:
                return false;
        }
    }

    public override string ToString()
    {
        switch (Source)
        {
            case InputSource.Keyboard:
                return Keyboard!.Value.ToString();
            case InputSource.Mouse:
                return Mouse!.Value.ToString();
            case InputSource.Gamepad:
                return Gamepad!.Value.ToString();
            case InputSource.None:
            default:
                return "?";
        }
    }
}
