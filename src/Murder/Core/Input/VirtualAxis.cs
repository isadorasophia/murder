using Murder.Utilities;
using Microsoft.Xna.Framework.Input;
using Murder.Core.Geometry;
using System.Collections.Immutable;

namespace Murder.Core.Input
{
    public class VirtualAxis : IVirtualInput
    {
        public Vector2 PreviousValue { get; private set; }
        public Point IntPreviousValue { get; private set; }
        public Vector2 Value { get; private set; }
        public Point IntValue { get; private set; }
        public Point PressedValue { get; private set; }

        public ImmutableArray<GamepadAxis> GamePadAxis = ImmutableArray.Create<GamepadAxis>();
        public ImmutableArray<KeyboardAxis> KeyboardAxis = ImmutableArray.Create<KeyboardAxis>();
        public ImmutableArray<ButtonAxis> ButtonAxis = ImmutableArray.Create<ButtonAxis>();

        public bool Pressed => Down && (IntValue != IntPreviousValue);
        public bool PressedX => Down && (IntValue.X != IntPreviousValue.X);
        public bool PressedY => Down && (IntValue.Y != IntPreviousValue.Y);
        internal bool Released => Previous && !Down;
        public bool Previous { get; private set; } = false;
        public bool Down { get; private set; } = false;
        public bool Consumed = false;

        public readonly float _deadZone = 0.1f;
        
        public event Action<InputState>? OnPress;

        public void Update(InputState inputState)
        {
            Previous = Down;
            Down = false;
            PreviousValue = Value;
            IntPreviousValue = IntValue;
            Value = Vector2.Zero;

            foreach (var a in KeyboardAxis)
            {
                var axis = ButtonToAxis(
                    inputState.KeyboardState.IsKeyDown(a.Up),
                    inputState.KeyboardState.IsKeyDown(a.Right),
                    inputState.KeyboardState.IsKeyDown(a.Left),
                    inputState.KeyboardState.IsKeyDown(a.Down));

                if (axis.HasValue)
                {
                    Down = true;
                    Game.Input.UsingKeyboard = true;
                    Value += axis;
                }
            }

            foreach (var a in GamePadAxis)
            {
                var axis = GetAxis(a, inputState.GamePadState);
                if (axis.HasValue)
                {
                    Down = true;
                    Game.Input.UsingKeyboard = false;
                    Value += axis;
                }
                if (Math.Abs(Value.X) < _deadZone)
                    Value = new(0,Value.Y);
                if (Math.Abs(Value.Y) < _deadZone)
                    Value = new(Value.X, 0);
            }

            foreach (var a in ButtonAxis)
            {
                var axis = ButtonToAxis(
                    inputState.GamePadState.IsButtonDown(a.Up),
                    inputState.GamePadState.IsButtonDown(a.Right),
                    inputState.GamePadState.IsButtonDown(a.Left),
                    inputState.GamePadState.IsButtonDown(a.Down));

                if (axis.HasValue)
                {
                    Down = true;
                    Game.Input.UsingKeyboard = false;
                    Value += axis;
                }
            }

            var lenghtSq = Value.LengthSquared();
            if (lenghtSq > 1)
                Value = Value.Normalized();

            IntValue = new Point(Calculator.PolarSnapToInt(Value.X), Calculator.PolarSnapToInt(Value.Y));

            if (Pressed)
            {
                PressedValue = new Point(MathF.Sign(Value.X), MathF.Sign(Value.Y));
                OnPress?.Invoke(inputState);
            }
            else
            {
                PressedValue = Point.Zero;
            }

            Consumed = false;
        }

        internal string GetDescriptor()
        {
            return StringHelper.ToHumanList(GetActiveButtonDescriptions(), ",", "or");
        }

        public IEnumerable<string> GetActiveButtonDescriptions()
        {
            var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);

            if (capabilities.IsConnected && !Game.Input.UsingKeyboard)
            {
                foreach (var axis in GamePadAxis)
                {
                    yield return axis.GetDescription();
                }
                foreach (var btn in ButtonAxis)
                {
                    yield return btn.ToString();
                }
            }
            else
            {
                foreach (var btn in KeyboardAxis)
                {
                    yield return btn.ToString();
                }
            }
        }

        private Vector2 GetAxis(GamepadAxis axis, GamePadState gamepadState)
        {
            switch (axis)
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
                    throw new Exception($"Gamepad axis '{axis}' is not supported yet.");
            }
        }

        public Vector2 ButtonToAxis(bool up, bool right, bool left, bool down)
        {
            int x = right ? 1 : 0;
            int y = down ? 1 : 0;
            x -= left ? 1 : 0;
            y -= up ? 1 : 0;

            return new(x, y);
        }

        internal void Consume()
        {
            Consumed = true;
        }
    }


    public readonly struct ButtonAxis
    {
        public readonly Buttons Up;
        public readonly Buttons Left;
        public readonly Buttons Down;
        public readonly Buttons Right;

        public ButtonAxis(Buttons up, Buttons left, Buttons down, Buttons right)
        {
            Up = up;
            Left = left;
            Down = down;
            Right = right;
        }

        public override string ToString()
        {
            var buttons = new string[] { Up.ToString(), Left.ToString(), Down.ToString(), Right.ToString() };
            return String.Join(", ", buttons);
        }
    }

    public readonly struct KeyboardAxis
    {
        public readonly Keys Up;
        public readonly Keys Left;
        public readonly Keys Down;
        public readonly Keys Right;

        public KeyboardAxis(Keys up, Keys left, Keys down, Keys right)
        {
            Up = up;
            Left = left;
            Down = down;
            Right = right;
        }

        public override string ToString()
        {
            var buttons = new string[] { Up.ToString(), Left.ToString(), Down.ToString(), Right.ToString() };
            return String.Join(", ", buttons);
        }
    }
}
