using System.ComponentModel;

namespace Murder.Core.Input
{
    public enum GamepadAxis
    {
        [Description("Left Thumbstick")]
        LeftThumb,

        [Description("Right Thumbstick")]
        RightThumb,

        [Description("D-Pad")]
        Dpad
    }
}