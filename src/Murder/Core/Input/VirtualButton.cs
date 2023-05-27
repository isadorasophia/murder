using Microsoft.Xna.Framework.Input;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Input
{
    public class VirtualButton : IVirtualInput
    {
        public ImmutableArray<MouseButtons> MouseButtons = ImmutableArray.Create<MouseButtons>();
        public ImmutableArray<Keys> Keyboard = ImmutableArray.Create<Keys>();
        public ImmutableArray<Buttons> Buttons = ImmutableArray.Create<Buttons>();

        public bool Pressed => Down && !Previous && !Consumed;
        internal bool Released => Previous && !Down;
        public bool Previous { get; private set; } = false;
        public bool Down { get; private set; } = false;
        public bool Consumed = false;

        public float LastPressed = 0f;
        public float LastReleased = 0f;
        public event Action<InputState>? OnPress;

        private const float TRIGGER_DEADZONE = 0.35f;

        public void Update(InputState inputState)
        {
            Previous = Down;
            Down = false;

            foreach (var k in Keyboard)
            {
                if (inputState.KeyboardState.IsKeyDown(k))
                {
                    Game.Input.UsingKeyboard = true;
                    Down = true;
                    break;
                }
            }
                
            if (!Down)
            {
                foreach (var b in Buttons)
                {
                    if (b == Microsoft.Xna.Framework.Input.Buttons.RightTrigger)
                    {
                        if (inputState.GamePadState.Triggers.Right > TRIGGER_DEADZONE)
                        {
                            Game.Input.UsingKeyboard = false;
                            Down = true;
                            break;
                        }
                    }
                    else if(b == Microsoft.Xna.Framework.Input.Buttons.LeftTrigger)
                    {
                        if (inputState.GamePadState.Triggers.Left > TRIGGER_DEADZONE)
                        {
                            Game.Input.UsingKeyboard = false;
                            Down = true;
                            break;
                        }
                    }
                    else if (inputState.GamePadState.IsButtonDown(b))
                    {
                        Game.Input.UsingKeyboard = false;
                        Down = true;
                        break;
                    }
                }
            }

            if (!Down)
            {
                foreach (var b in MouseButtons)
                {
                    if (IsMouseCurrentlyPressed(inputState.MouseState, b))
                    {
                        Down = true;
                        break;
                    }
                }

                Consumed = false;
            }

            if (Pressed)
            {
                OnPress?.Invoke(inputState);
                LastPressed = Game.NowUnescaled;
            }

            if (Released){
                LastReleased = Game.NowUnescaled;
            }
        }

        public string GetDescriptor()
        {
            return StringHelper.ToHumanList(GetActiveDescriptors(), ",", "or");
        }

        private IEnumerable<string> GetActiveDescriptors()
        {
            var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);

            if (capabilities.IsConnected && !Game.Input.UsingKeyboard)
            {
                foreach (var btn in Buttons)
                {
                    yield return btn.ToString();
                }
            }
            else
            {
                foreach (var btn in MouseButtons)
                {
                    yield return btn.ToString();
                }
                foreach (var btn in Keyboard)
                {
                    yield return btn.ToString();
                }
            }
        }

        internal void ClearBinds()
        {
            OnPress = default;
        }

        internal void Consume()
        {
            Consumed = true;
        }

        private bool IsMouseCurrentlyPressed(MouseState mouseState, MouseButtons button)
            => button switch
            {
                Input.MouseButtons.Left => mouseState.LeftButton is ButtonState.Pressed,
                Input.MouseButtons.Middle => mouseState.MiddleButton is ButtonState.Pressed,
                Input.MouseButtons.Right => mouseState.RightButton is ButtonState.Pressed,
                _ => false
            };
    }
}
