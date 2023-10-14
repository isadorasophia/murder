
namespace Murder.Core.Input;

public enum InputImageStyle
{
    None,
    Keyboard,
    KeyboardLong,
    GamepadButtonGeneric,
    GamepadButtonNorth,
    GamepadButtonSouth,
    GamepadButtonEast,
    GamepadButtonWest,
    GamepadStick,
    GamepadLeftShoulder,
    GamepadRightShoulder,
    GamepadDPad,
    GamepadDPadUp,
    GamepadDPadDown,
    GamepadDPadLeft,
    GamepadDPadRight,
    GamepadExtra,
    MouseLeft,
    MouseRight,
    MouseMiddle,
    MouseWheel,
    MouseExtra,
}

public enum InputSource
{
    None = -1,
    Keyboard = 0,
    Mouse = 1,
    Gamepad = 2,
    GamepadAxis = 3
}