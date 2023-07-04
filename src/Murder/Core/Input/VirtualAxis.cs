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

        public ImmutableArray<InputButtonAxis> ButtonAxis = ImmutableArray.Create<InputButtonAxis>();
        public ImmutableArray<InputButton> Axis = ImmutableArray.Create<InputButton>();

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

            // Check for input button axes. Which are just axes
            // made of individual InputButtons
            foreach (var a in ButtonAxis)
            {
                var axis = ButtonToAxis(
                    a.Up.Check(inputState),
                    a.Right.Check(inputState),
                    a.Left.Check(inputState),
                    a.Down.Check(inputState));

                if (axis.HasValue)
                {
                    Down = true;
                    Game.Input.UsingKeyboard = true;
                    Value += axis;
                }
            }

            // Now check for any Gamepad axes
            foreach (var a in Axis)
            {
                var axis = a.GetAxis(inputState.GamePadState);
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

            var lengthSq = Value.LengthSquared();
            if (lengthSq > 1)
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
            foreach (var btn in ButtonAxis)
            {
                yield return btn.ToString();
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
    
}
