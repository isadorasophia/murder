
using Microsoft.Xna.Framework.Input;

namespace Murder.Core.Input;

public readonly struct InputButton
{
    public readonly InputSource Source = InputSource.None;
    public readonly Keys? Keyboard = null;
    public readonly Buttons? Gamepad = null;
    public readonly MouseButtons? Mouse = null;

    public InputButton() { }
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

    public bool IsAvailable(GamePadCapabilities capabilities)
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
    
    public InputImageStyle GetInputImageStyle()
    {
        switch (Source)
        {
            case InputSource.Keyboard:
                switch (Keyboard!.Value)
                {
                    case Keys.Tab:
                    case Keys.Enter:
                    case Keys.Space:
                    case Keys.Escape:
                        return InputImageStyle.KeyboardLong;
                    default:
                        return InputImageStyle.Keyboard;
                }
            case InputSource.Mouse:
                switch (Mouse!.Value)
                {
                    case MouseButtons.Left:
                        return InputImageStyle.MouseLeft;
                    case MouseButtons.Middle:
                        return InputImageStyle.MouseMiddle;
                    case MouseButtons.Right:
                        return InputImageStyle.MouseRight;
                    default:
                        return InputImageStyle.MouseExtra;
                }

            case InputSource.Gamepad:
                switch (Gamepad!.Value)
                {
                    case Buttons.DPadUp:
                        return InputImageStyle.GamepadDPadUp;
                    case Buttons.DPadDown:
                        return InputImageStyle.GamepadDPadDown;
                    case Buttons.DPadLeft:
                        return InputImageStyle.GamepadDPadLeft;
                    case Buttons.DPadRight:
                        return InputImageStyle.GamepadDPadRight;
                    case Buttons.Start:
                        return InputImageStyle.GamepadExtra;
                    case Buttons.Back:
                        return InputImageStyle.GamepadExtra;
                    case Buttons.LeftThumbstickUp:
                    case Buttons.LeftThumbstickDown:
                    case Buttons.LeftThumbstickLeft:
                    case Buttons.LeftThumbstickRight:
                    case Buttons.LeftStick:
                        return InputImageStyle.GamepadStick;
                    case Buttons.RightThumbstickUp:
                    case Buttons.RightThumbstickDown:
                    case Buttons.RightThumbstickLeft:
                    case Buttons.RightThumbstickRight:
                    case Buttons.RightStick:
                        return InputImageStyle.GamepadStick;
                    case Buttons.LeftTrigger:
                    case Buttons.LeftShoulder:
                        return InputImageStyle.GamepadLeftShoulder;
                    case Buttons.RightTrigger:
                    case Buttons.RightShoulder:
                        return InputImageStyle.GamepadRightShoulder;
                    case Buttons.BigButton:
                        return InputImageStyle.GamepadExtra;
                    case Buttons.A:
                        return InputImageStyle.GamepadButtonEast;
                    case Buttons.B:
                        return InputImageStyle.GamepadButtonSouth;
                    case Buttons.X:
                        return InputImageStyle.GamepadButtonNorth;
                    case Buttons.Y:
                        return InputImageStyle.GamepadButtonWest;
                    default:
                        return InputImageStyle.GamepadButtonGeneric;
                }
            case InputSource.None:
            default:
                return InputImageStyle.None;
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
