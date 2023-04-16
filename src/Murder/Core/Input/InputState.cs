using Microsoft.Xna.Framework.Input;

namespace Murder.Core.Input
{
    // TODO: Implement service that provides scale where the cursor currently is.
    public readonly struct InputState
    {
        //public readonly GamePadCapabilities GamePadCapabilities;
        public readonly KeyboardState KeyboardState;

        public readonly MouseState MouseState;
        public readonly GamePadState GamePadState;

        public InputState(KeyboardState keyboardState, GamePadState gamePadState, MouseState mouseState)//, GamePadCapabilities gamePadCapabilities)
        {
            KeyboardState = keyboardState;
            GamePadState = gamePadState;
            MouseState = mouseState;
            //GamePadCapabilities = gamePadCapabilities;
        }
    }
}