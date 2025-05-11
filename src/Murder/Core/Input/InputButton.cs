using Bang;
using Microsoft.Xna.Framework.Input;
using System.Numerics;

namespace Murder.Core.Input;

public readonly struct InputButton
{
    public readonly InputSource Source = InputSource.None;
    [Serialize]
    private readonly Keys? _keyboard = null;
    [Serialize]
    private readonly Buttons? _gamepad = null;
    [Serialize]
    private readonly MouseButtons? _mouse = null;
    [Serialize]
    private readonly GamepadAxis? _axis = null;

    public Keys? Keyboard => _keyboard;
    public Buttons? Gamepad => _gamepad;
    public MouseButtons? Mouse => _mouse;
    public GamepadAxis? Axis => _axis;

    public InputButton() { }
    public InputButton(Keys key)
    {
        Source = InputSource.Keyboard;
        _keyboard = key;
    }

    public InputButton(Buttons button)
    {
        Source = InputSource.Gamepad;
        _gamepad = button;
    }

    public InputButton(MouseButtons button)
    {
        Source = InputSource.Mouse;
        _mouse = button;
    }
    public InputButton(GamepadAxis axis)
    {
        Source = InputSource.GamepadAxis;
        _axis = axis;
    }

    public InputButton(InputSource source, Keys? keys, Buttons? buttons, MouseButtons? mouseButtons, GamepadAxis? gamepadAxis) : this()
    {
        Source = source;
        _keyboard = keys;
        _gamepad = buttons;
        _mouse = mouseButtons;
        _axis = gamepadAxis;
    }

    public Vector2 GetAxis(GamePadState gamepadState)
    {
        if (Source == InputSource.GamepadAxis)
        {
            switch (_axis!.Value)
            {
                case GamepadAxis.LeftThumb:
                    return new(gamepadState.ThumbSticks.Left.X, -gamepadState.ThumbSticks.Left.Y);
                case GamepadAxis.RightThumb:
                    return new(gamepadState.ThumbSticks.Right.X, -gamepadState.ThumbSticks.Right.Y);
                case GamepadAxis.Dpad:
                    return ButtonToAxis(
                        gamepadState.DPad.Up == ButtonState.Pressed,
                        gamepadState.DPad.Right == ButtonState.Pressed,
                        gamepadState.DPad.Left == ButtonState.Pressed,
                        gamepadState.DPad.Down == ButtonState.Pressed);
                default:
                    throw new Exception($"Gamepad axis '{_axis}' is not supported yet.");
            }
        }

        return Vector2.Zero;
    }

    public Vector2 ButtonToAxis(bool up, bool right, bool left, bool down)
    {
        int x = right ? 1 : 0;
        int y = down ? 1 : 0;
        x -= left ? 1 : 0;
        y -= up ? 1 : 0;

        return new(x, y);
    }


    public bool Check(InputState state)
    {
        switch (Source)
        {
            case InputSource.None:
                return false;
            case InputSource.Keyboard:
                return state.KeyboardState.IsKeyDown(_keyboard!.Value);
            case InputSource.Gamepad:
                return state.GamePadState.IsButtonDown(_gamepad!.Value);
            case InputSource.Mouse:
                switch (_mouse!.Value)
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
                switch (_keyboard!.Value)
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
                switch (_mouse!.Value)
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
                switch (_gamepad!.Value)
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
                return _keyboard?.ToString() ?? "<none>";
            case InputSource.Mouse:
                return _mouse?.ToString() ?? "<none>";
            case InputSource.Gamepad:
                return _gamepad?.ToString() ?? "<none>";
            case InputSource.None:
            default:
                return "?";
        }
    }
}