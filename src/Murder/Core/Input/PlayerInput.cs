using Microsoft.Xna.Framework.Input;
using Murder.Core.Geometry;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Input
{
    public class PlayerInput
    {
        private readonly Dictionary<InputButtons, VirtualButton> _buttons = new();
        private readonly Dictionary<InputAxis, VirtualAxis> _axis = new();

        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;

        public Point CursorPosition;
        internal bool UsingKeyboard = false;

        /// <summary>
        /// Scrollwheel delta
        /// </summary>
        public int ScrollWheel => _previousScrollWheel - _scrollWheel;
        private int _scrollWheel = 0;
        private int _previousScrollWheel = 0;

        private bool _lockInputs = false;

        public VirtualButton GetOrCreateButton(InputButtons button)
        {
            if (!_buttons.ContainsKey(button) || _buttons[button] == null)
            {
                _buttons[button] = new VirtualButton();
                //GameDebugger.Log($"Creating a VirtualButton called '{button}'");
            }

            return _buttons[button];
        }

        internal string GetDescriptor(InputAxis movement)
        {
            return GetOrCreateAxis(movement).GetDescriptor();
        }

        internal string GetDescriptor(InputButtons button)
        {
            return GetOrCreateButton(button).GetDescriptor();
        }

        public VirtualAxis GetOrCreateAxis(InputAxis axis)
        {
            if (!_axis.ContainsKey(axis) || _axis[axis] == null)
            {
                _axis[axis] = new VirtualAxis();
                //GameDebugger.Log($"Creating a VirtualButton called '{button}'");
            }

            return _axis[axis];
        }

        /// <summary>
        /// Lock <see cref="InputButtons"/> queries and do not propagate then to the game.
        /// </summary>
        public void Lock(bool value)
        {
            _lockInputs = value;
        }

        public void Register(InputAxis axis, params KeyboardAxis[] keyboardAxes)
        { 
            var a = GetOrCreateAxis(axis);
            a.KeyboardAxis = keyboardAxes.ToImmutableArray();
        }

        public void Register(InputAxis axis, params ButtonAxis[] buttonAxes)
        {
            var a = GetOrCreateAxis(axis);
            a.ButtonAxis = buttonAxes.ToImmutableArray();
        }

        public void Register(InputAxis axis, params GamepadAxis[] gamepadAxis)
        {
            var a = GetOrCreateAxis(axis);
            a.GamePadAxis = gamepadAxis.ToImmutableArray();
        }

        public void Register(InputButtons button, params Keys[] keys)
        {
            var b = GetOrCreateButton(button);
            b.Keyboard = keys.ToImmutableArray();
        }

        public void Register(InputButtons button, params Buttons[] buttons)
        {
            var b = GetOrCreateButton(button);
            b.Buttons = buttons.ToImmutableArray();
        }

        public void ClearBinds(InputButtons button)
        {
            var b = GetOrCreateButton(button);
            b.ClearBinds();
        }

        public void Register(InputButtons button, params MouseButtons[] keys)
        {
            var b = GetOrCreateButton(button);
            b.MouseButtons = keys.ToImmutableArray();
        }

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            bool gamepadAvailable = false;
            if (GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsConnected)
            {
                var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);
                gamepadAvailable = capabilities.IsConnected && capabilities.GamePadType == GamePadType.GamePad;
            }

            GamePadState gamepadState = gamepadAvailable ? GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One) : new();
            
            InputState inputState = new(_currentKeyboardState, gamepadState, Mouse.GetState());

            var scale = Game.Instance.GameScale;
            CursorPosition = new (
                Calculator.RoundToInt(inputState.MouseState.Position.X),
                Calculator.RoundToInt(inputState.MouseState.Position.Y));

            if (_lockInputs)
            {
                _buttons[InputButtons.Debug].Update(inputState);
            }
            else
            {
                foreach (var button in _buttons)
                {
                    button.Value.Update(inputState);
                }

                foreach (var axis in _axis)
                {
                    axis.Value.Update(inputState);
                }
            }

            _previousScrollWheel = _scrollWheel;
            _scrollWheel = inputState.MouseState.ScrollWheelValue;
        }

        public void Bind(InputButtons button, Action<InputState> action)
        {
            GetOrCreateButton(button).OnPress += action;
        }

        public bool Shortcut(Keys key, params Keys[] modifiers)
        { 
            var keyboardState = Keyboard.GetState();
            foreach (var k in modifiers)
            {
                if (!keyboardState.IsKeyDown(k))
                    return false;
            }

            if (!_previousKeyboardState.IsKeyDown(key) && keyboardState.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }

        public bool Released(InputButtons button)
        {
            return _buttons[button].Released;
        }

        public bool Pressed(Keys enter)
        {
            return Keyboard.GetState().IsKeyDown(enter);
        }

        internal bool PressedAndConsume(InputButtons button)
        {
            if (Pressed(button))
            {
                Consume(button);
                return true;
            }
            return false;
        }

        public void Consume(InputButtons button)
        {
            if (_buttons.TryGetValue(button, out VirtualButton? virtualButton))
            {
                virtualButton.Consume();
            }
        }

        public VirtualAxis GetAxis(InputAxis axis, bool raw = false)
        {
            if (_axis.TryGetValue(axis, out var a))
            {
                return a;
            }

            throw new Exception($"Couldn't find button of type {axis}");
        }

        public bool Pressed(InputButtons button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Pressed && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        }
        public bool Down(InputButtons button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Down && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        } 

        internal bool Released(InputButtons button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Released && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        }
    }
}
