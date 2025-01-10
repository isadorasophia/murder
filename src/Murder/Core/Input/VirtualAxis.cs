using Microsoft.Xna.Framework.Input;
using Murder.Core.Geometry;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Core.Input
{
    public class VirtualAxis : IVirtualInput
    {
        public Vector2 PreviousValue { get; private set; }
        public Point IntPreviousValue { get; private set; }
        public Vector2 Value { get; private set; }
        public Point IntValue { get; private set; }
        public Point PressedValue { get; private set; }

        public ImmutableArray<InputButtonAxis> ButtonAxis => _buttonAxis;
        private ImmutableArray<InputButtonAxis> _buttonAxis = ImmutableArray.Create<InputButtonAxis>();

        public InputButtonAxis?[] _lastPressedButton = new InputButtonAxis?[2];

        public bool Pressed => Down && (IntValue != IntPreviousValue);
        public bool PressedX => Down && (IntValue.X != IntPreviousValue.X);

        /// <summary>
        /// Like a keyboard, 1 when pressed and then every <see cref="_tickDelay"/>. First tick is <see cref="_firstTickDelay"/>.
        /// </summary>
        public Point Tick => new Point(_tickX ? IntValue.X : 0, _tickY ? IntValue.Y : 0);

        /// <summary>
        /// Like a keyboardkey, true on pressed and then every <see cref="_tickDelay"/>. First tick is <see cref="_firstTickDelay"/>.
        /// </summary>
        public bool TickX => _tickX;
        public bool PressedY => Down && (IntValue.Y != IntPreviousValue.Y);
        /// <summary>
        /// Like a keyboardkey, true on pressed and then every <see cref="_tickDelay"/>. First tick is <see cref="_firstTickDelay"/>.
        /// </summary>
        public bool TickY => _tickY;
        internal bool Released => Previous && !Down;
        public bool Previous { get; private set; } = false;
        public bool Down { get; private set; } = false;
        public bool Consumed = false;

        private readonly float _firstTickDelay = 0.3f;
        private readonly float _tickDelay = 0.15f;

        public event Action<InputState>? OnPress;

        private float _pressedYStart;
        private float _pressedXStart;

        private float _nextYTick;
        private float _nextXTick;

        private bool _tickX;
        private bool _tickY;

        public void Update(InputState inputState)
        {
            Previous = Down;
            Down = false;
            PreviousValue = Value;
            IntPreviousValue = IntValue;
            Value = Vector2.Zero;

            // Check for input button axes. Which are just axes
            // made of individual InputButtons
            foreach (var axis in _buttonAxis)
            {
                var vector = axis.Check(inputState);

                if (vector.HasValue())
                {
                    Down = true;
                    Game.Input.UsingKeyboard = (axis.Source == InputSource.Keyboard || axis.Source == InputSource.Mouse);
                    _lastPressedButton[Game.Input.UsingKeyboard ? 1 : 0] = axis;
                    Value += vector;
                }
            }

            var lengthSq = Value.LengthSquared();
            if (lengthSq > 1)
                Value = Value.Normalized();

            IntValue = new Point(Calculator.PolarSnapToInt(Value.X), Calculator.PolarSnapToInt(Value.Y));

            if (Pressed)
            {
                PressedValue = new Point(MathF.Sign(Value.X), MathF.Sign(Value.Y));
                if (PressedX)
                {
                    _pressedXStart = Game.NowUnscaled;
                    _nextXTick = Game.NowUnscaled + _firstTickDelay;
                    _tickX = true;
                }
                if (PressedY)
                {
                    _pressedYStart = Game.NowUnscaled;
                    _nextYTick = Game.NowUnscaled + _firstTickDelay;
                    _tickY = true;
                }

                OnPress?.Invoke(inputState);
            }
            else
            {
                PressedValue = Point.Zero;

                if (Value.X != 0)
                {
                    if (!_tickX && _nextXTick < Game.NowUnscaled)
                    {
                        _nextXTick = Game.NowUnscaled + _tickDelay;
                        _tickX = true;
                    }
                    else
                    {
                        _tickX = false;
                    }
                }

                if (Value.Y != 0)
                {
                    if (!_tickY && _nextYTick < Game.NowUnscaled)
                    {
                        _nextYTick = Game.NowUnscaled + _tickDelay;
                        _tickY = true;
                    }
                    else
                    {
                        _tickY = false;
                    }
                }
            }

            Consumed = false;
        }

        public void Press(Vector2 value)
        {
            PreviousValue = Value;
            IntPreviousValue = IntValue;

            Down = true;
            Consumed = false;

            Value = value;
            PressedValue = new Point(MathF.Sign(Value.X), MathF.Sign(Value.Y));
            IntValue = new Point(Calculator.PolarSnapToInt(Value.X), Calculator.PolarSnapToInt(Value.Y));

            if (PressedX)
            {
                _pressedXStart = Game.NowUnscaled;
                _nextXTick = Game.NowUnscaled + _firstTickDelay;
                _tickX = true;
            }

            if (PressedY)
            {
                _pressedYStart = Game.NowUnscaled;
                _nextYTick = Game.NowUnscaled + _firstTickDelay;
                _tickY = true;
            }
        }

        internal string GetDescriptor()
        {
            return StringHelper.ToHumanList(GetActiveButtonDescriptions(), ",", "or");
        }

        public IEnumerable<string> GetActiveButtonDescriptions()
        {
            var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);
            foreach (var btn in _buttonAxis)
            {
                yield return btn.ToString();
            }
        }

        internal void Consume()
        {
            Consumed = true;
        }

        internal void Register(InputButtonAxis[] buttonAxes)
        {
            _buttonAxis = _buttonAxis.AddRange(buttonAxes);
        }

        internal void Register(GamepadAxis[] gamepadAxis)
        {
            var builder = ImmutableArray.CreateBuilder<InputButtonAxis>();
            for (int i = 0; i < gamepadAxis.Length; i++)
            {
                builder.Add(new InputButtonAxis(gamepadAxis[i]));
            }
            _buttonAxis = _buttonAxis.AddRange(builder.ToImmutableArray());
        }


        public InputButtonAxis LastPressedAxes(bool keyboard)
        {
            if (_lastPressedButton[keyboard ? 1 : 0] is InputButtonAxis button)
            {
                return button;
            }

            foreach (var b in _buttonAxis)
            {
                if (b.Source == InputSource.Gamepad && keyboard)
                    continue;

                if ((b.Source == InputSource.Mouse || b.Source == InputSource.Keyboard) && !keyboard)
                    continue;

                return b;
            }

            return _buttonAxis.FirstOrDefault();
        }
    }

}