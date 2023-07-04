using Microsoft.Xna.Framework.Input;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace Murder.Core.Input
{
    public class PlayerInput
    {
        public int[] AllButtons => _buttons.Keys.ToArray();
        public int[] AllAxis => _axis.Keys.ToArray();
        private readonly Dictionary<int, VirtualButton> _buttons = new();
        private readonly Dictionary<int, VirtualAxis> _axis = new();

        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;

        public Point CursorPosition;
        public bool UsingKeyboard = false;

        public bool MouseConsumed = false;

        /// <summary>
        /// Scrollwheel delta
        /// </summary>
        public int ScrollWheel
        {
            get
            {
                return _previousScrollWheel - _scrollWheel;
            }
        }

        private int _scrollWheel = 0;
        private int _previousScrollWheel = 0;

        private float _lastUpdateTime;

        private bool _lockInputs = false;

        public VirtualButton GetOrCreateButton(int button)
        {
            if (!_buttons.ContainsKey(button) || _buttons[button] == null)
            {
                _buttons[button] = new VirtualButton();
                //GameDebugger.Log($"Creating a VirtualButton called '{button}'");
            }

            return _buttons[button];
        }

        public string GetAxisDescriptor(int axis)
        {
            return GetOrCreateAxis(axis).GetDescriptor();
        }

        public string GetButtonDescriptor(int button)
        {
            return GetOrCreateButton(button).GetDescriptor();
        }

        public VirtualAxis GetOrCreateAxis(int axis)
        {
            if (!_axis.ContainsKey(axis) || _axis[axis] == null)
            {
                _axis[axis] = new VirtualAxis();
                //GameDebugger.Log($"Creating a VirtualButton called '{button}'");
            }

            return _axis[axis];
        }

        /// <summary>
        /// Lock <see cref="_buttons"/> queries and do not propagate then to the game.
        /// </summary>
        public void Lock(bool value)
        {
            _lockInputs = value;
        }

        public void Register(int axis, params KeyboardAxis[] keyboardAxes)
        {
            var a = GetOrCreateAxis(axis);
            a.KeyboardAxis = keyboardAxes.ToImmutableArray();
        }

        public void Register(int axis, params ButtonAxis[] buttonAxes)
        {
            var a = GetOrCreateAxis(axis);
            a.ButtonAxis = buttonAxes.ToImmutableArray();
        }

        public void Register(int axis, params GamepadAxis[] gamepadAxis)
        {
            var a = GetOrCreateAxis(axis);
            a.GamePadAxis = gamepadAxis.ToImmutableArray();
        }

        public void Register(int button, params Keys[] keys)
        {
            var b = GetOrCreateButton(button);
            b.Register(keys);
        }

        public void Register(int button, params Buttons[] buttons)
        {
            var b = GetOrCreateButton(button);
            b.Register(buttons);
        }

        public void ClearBinds(int button)
        {
            var b = GetOrCreateButton(button);
            b.ClearBinds();
        }

        public void Register(int button, params MouseButtons[] buttons)
        {
            var b = GetOrCreateButton(button);
            b.Register(buttons);
        }

        public void Update()
        {
            // Maybe we need to use just Fixed Delta Time here. Trying 1000x for extra precision.
            if (Game.NowUnescaled - _lastUpdateTime < Game.FixedDeltaTime / 1000f)
                return;
            _lastUpdateTime = Game.NowUnescaled;

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
            CursorPosition = new(
                Calculator.RoundToInt(inputState.MouseState.Position.X),
                Calculator.RoundToInt(inputState.MouseState.Position.Y));

#if DEBUG
            if (MouseConsumed || _lockInputs)
            {
                _buttons[MurderInputButtons.Debug].Update(inputState);
            }
            else
#endif
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

        public void Bind(int button, Action<InputState> action)
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

        public bool Released(int button)
        {
            return _buttons[button].Released;
        }

        public bool Pressed(Keys enter)
        {
            return Keyboard.GetState().IsKeyDown(enter);
        }

        public bool PressedAndConsume(int button)
        {
            if (Pressed(button))
            {
                Consume(button);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consumes all buttons that have anything in common with this
        /// </summary>
        /// <param name="button"></param>
        public void Consume(int button)
        {
            if (_buttons.TryGetValue(button, out VirtualButton? virtualButton))
            {
                virtualButton.Consume();

                foreach (var otherVirtualButtonPair in _buttons)
                {
                    if (otherVirtualButtonPair.Value is VirtualButton other)
                    {
                        if (other.Consumed) continue;

                        // Check all gamepad buttons
                        foreach (var value in virtualButton.Buttons)
                        {
                            if (other.Buttons.Contains(value))
                            {
                                other.Consume();
                            }
                        }
                        if (other.Consumed) continue;
                        
                    }
                }
            }
        }

        public void ConsumeAll()
        {
            foreach (var button in _buttons)
            {
                button.Value.Consume();
            }

            foreach (var axis in _axis)
            {
                axis.Value.Consume();
            }
        }

        public VirtualAxis GetAxis(int axis)
        {
            if (_axis.TryGetValue(axis, out var a))
            {
                return a;
            }

            throw new Exception($"Couldn't find button of type {axis}");
        }

        public bool Pressed(int button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Pressed && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        }

        public bool Down(int button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Down && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        }

        internal bool Released(int button, bool raw = false)
        {
            if (_buttons.TryGetValue(button, out var btn))
            {
                return btn.Released && (raw || !btn.Consumed);
            }

            throw new Exception($"Couldn't find button of type {button}");
        }

        public bool HorizontalMenu(ref int selectedOption, int length)
        {
            int move = 0;
            var axis = GetAxis(MurderInputAxis.Ui);
            if (axis.Pressed)
            {
                move = Math.Sign(axis.Value.X);
            }

            selectedOption = Calculator.WrapAround(selectedOption + move, 0, length - 1);

            return PressedAndConsume(MurderInputButtons.Submit);
        }
        
        public bool HorizontalMenu(ref MenuInfo currentInfo)
        {
            if (currentInfo.Disabled)
            {
                return false;
            }

            VirtualAxis axis = GetAxis(MurderInputAxis.Ui);
            return HorizontalOrVerticalMenu(ref currentInfo, input: axis.PressedX ? Math.Sign(axis.Value.X) : null,
                overflow: axis.PressedY ? axis.IntValue.Y : 0);
        }

        public bool VerticalMenu(ref MenuInfo currentInfo)
        {
            if (currentInfo.Disabled)
            {
                return false;
            }

            currentInfo.JustMoved = false;

            VirtualAxis axis = GetAxis(MurderInputAxis.Ui);
            return HorizontalOrVerticalMenu(ref currentInfo, axis.PressedY ? Math.Sign(axis.Value.Y) : null,
                axis.PressedX ? axis.IntValue.X : 0);
        }

        private bool HorizontalOrVerticalMenu(ref MenuInfo currentInfo, float? input, int overflow)
        {   
            bool pressed = false;
            if (Pressed(MurderInputButtons.Submit))
            {
                currentInfo.LastPressed = Game.NowUnescaled;
                pressed = true;
            }

            bool canceled = false;
            if (Pressed(MurderInputButtons.Cancel))
            {
                canceled = true;
            }

            currentInfo.Canceled = canceled;
            currentInfo.Overflow = overflow;

            if (currentInfo.Disabled || currentInfo.Options == null || currentInfo.Length ==0)
                return false;

            if (pressed)
            {
                Consume(MurderInputButtons.Submit);
            }

            if (input is not null)
            {
                // Pick the next option. However, we need to take into account options that can't be selected,
                // so this gets slightly trickier.
                int sign = Math.Sign(input.Value);

                int newOption = currentInfo.NextAvailableOption(currentInfo.Selection, sign);
                if (newOption != currentInfo.Selection)
                {
                    currentInfo.Select(newOption, Game.NowUnescaled);
                }
            }

            return pressed;
        }

        public bool SimpleVerticalMenu(ref int selectedOption, int length)
        {
            int move = 0;
            var axis = GetAxis(MurderInputAxis.Ui);
            if (axis.PressedY)
            {
                move = Math.Sign(axis.Value.Y);
            }
            
            selectedOption = Calculator.WrapAround(selectedOption + move, 0, length - 1);

            return PressedAndConsume(MurderInputButtons.Submit);
        }

        [Flags]
        public enum GridMenuFlags
        {
            None,
            ClampRight,
            ClampLeft,
            ClampTop,
            ClampBottom,
            ClampAll
        }

        public bool GridMenu(ref MenuInfo currentInfo, int width, int _, int size, GridMenuFlags gridMenuFlags = GridMenuFlags.None)
        {
            if (currentInfo.Disabled)
                return false;

            var axis = GetAxis(MurderInputAxis.Ui);
            float lastMoved = currentInfo.LastMoved;
            float lastPressed = currentInfo.LastPressed;

            // Recalculate height based on the size.
            int height = Calculator.CeilToInt((float)size / width);
            int lastRowWidth = width - (width * height - size);

            int selectedOptionX = currentInfo.Selection % width;
            int selectedOptionY = Calculator.FloorToInt(currentInfo.Selection / width);
            int overflow = 0;
            if (axis.PressedX)
            {
                selectedOptionX += Math.Sign(axis.Value.X);

                int currentWidth = selectedOptionY == height - 1 ? lastRowWidth : width;

                if (selectedOptionX >= currentWidth) // Is on last row and it has less than width.
                {
                    overflow = 1;
                    if (gridMenuFlags.HasFlag(GridMenuFlags.ClampRight))
                        selectedOptionX = currentWidth - 1;
                }
                else if (selectedOptionX < 0)
                {
                    overflow = -1;
                    if (gridMenuFlags.HasFlag(GridMenuFlags.ClampLeft))
                        selectedOptionX = 0;
                }
                
                selectedOptionX = Calculator.WrapAround(selectedOptionX, 0, currentWidth - 1);

                lastMoved = Game.NowUnescaled;
            }

            if (axis.PressedY)
            {
                selectedOptionY += Math.Sign(axis.Value.Y);

                int currentHeight = selectedOptionX >= lastRowWidth ? height - 1 : height;

                if (selectedOptionY >= currentHeight && gridMenuFlags.HasFlag(GridMenuFlags.ClampBottom))
                {
                    selectedOptionY = currentHeight - 1;
                }
                else if (selectedOptionY < 0)
                {
                    selectedOptionY = 0;
                }
                
                selectedOptionY = Calculator.WrapAround(selectedOptionY, 0, currentHeight - 1);

                lastMoved = Game.NowUnescaled;
            }

            int selectedOptionIndex = selectedOptionX + selectedOptionY * width;

            bool pressed = false;
            if (PressedAndConsume(MurderInputButtons.Submit))
            {
                lastPressed = Game.NowUnescaled;
                pressed = true;
            }

            bool canceled = false;
            if (Pressed(MurderInputButtons.Cancel))
            {
                canceled = true;
            }
            
            currentInfo.Select(selectedOptionIndex, lastMoved);
            
            currentInfo.Canceled =  canceled;
            currentInfo.Overflow = overflow;
            return pressed;
        }

        private bool _registerKeyboardInputs = false;
        private int _maxCharacters = 32;

        private StringBuilder _userKeyboardInput = new();

        public void ClampText(int size)
        {
            if (size >= _userKeyboardInput.Length)
            {
                return;
            }

            _userKeyboardInput.Remove(size, _userKeyboardInput.Length - size);
        }

        public void ListenToKeyboardInput(bool enable, int maxCharacters = 32)
        {
            if (_registerKeyboardInputs == enable)
            {
                _userKeyboardInput = new();

                return;
            }

            if (enable)
            {
                Game.Instance.Window.TextInput += OnDesktopTextInput;
            }
            else
            {
                Game.Instance.Window.TextInput -= OnDesktopTextInput;
            }

            _userKeyboardInput = new();

            _registerKeyboardInputs = enable;
            _maxCharacters = maxCharacters;
        }

        public string GetKeyboardInput() => _userKeyboardInput.ToString();

        private void OnDesktopTextInput(object? sender, Microsoft.Xna.Framework.TextInputEventArgs args)
        {
            Keys key = args.Key;
            if (key == Keys.Back)
            {
                if (_userKeyboardInput.Length > 0)
                {
                    _userKeyboardInput.Remove(_userKeyboardInput.Length - 1, 1);
                }

                return;
            }
            else if (key == Keys.Enter || key == Keys.Escape)
            {
                return;
            }

            char c = args.Character;
            if (_userKeyboardInput.Length >= _maxCharacters)
            {
                return;
            }

            _userKeyboardInput.Append(c);
        }
    }
}