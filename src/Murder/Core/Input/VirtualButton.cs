using Microsoft.Xna.Framework.Input;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Core.Input;

public class VirtualButton : IVirtualInput
{
    public ImmutableArray<InputButton> Buttons = ImmutableArray<InputButton>.Empty;
    public InputButton?[] _lastPressedButton = new InputButton?[2];

    public bool Pressed => Down && !Previous && !Consumed;
    internal bool Released => Previous && !Down;
    public bool Previous { get; private set; } = false;
    public bool Down { get; private set; } = false;
    public bool Consumed = false;

    public float LastPressed = 0f;
    public float LastReleased = 0f;
    public event Action<InputState>? OnPress;

    public void Update(InputState inputState)
    {
        Previous = Down;
        Down = false;

        foreach (var button in Buttons)
        {
            if (button.Check(inputState))
            {
                Down = true;
                Game.Input.UsingKeyboard = (button.Source == InputSource.Keyboard || button.Source == InputSource.Mouse);
                _lastPressedButton[Game.Input.UsingKeyboard ? 1 : 0] = button;
                break;
            }
        }

        if (!Down)
        {
            Consumed = false;
        }

        if (Pressed)
        {
            OnPress?.Invoke(inputState);
            LastPressed = Game.NowUnscaled;
        }

        if (Released)
        {
            LastReleased = Game.NowUnscaled;
        }
    }

    /// <summary>
    /// Force pressing the button.
    /// </summary>
    public void Press()
    {
        Previous = false;
        Down = true;
        Consumed = false;

        LastPressed = Game.NowUnscaled;
    }

    public string GetDescriptor()
    {
        return StringHelper.ToHumanList(GetActiveDescriptors(), ",", "or");
    }

    private IEnumerable<string> GetActiveDescriptors()
    {
        var capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);

        if (capabilities.IsConnected)
        {
            foreach (var btn in Buttons)
            {
                if (btn.IsAvailable(capabilities))
                    yield return btn.ToString();
            }
        }
    }

    internal void ClearBinds()
    {
        OnPress = default;
    }

    public void Consume()
    {
        Consumed = true;
    }

    public void Free()
    {
        Consumed = false;
    }

    private bool IsMouseCurrentlyPressed(MouseState mouseState, MouseButtons button)
    {
        return button switch
        {
            Input.MouseButtons.Left => mouseState.LeftButton is ButtonState.Pressed,
            Input.MouseButtons.Middle => mouseState.MiddleButton is ButtonState.Pressed,
            Input.MouseButtons.Right => mouseState.RightButton is ButtonState.Pressed,
            _ => false
        };
    }

    public void Register(ButtonBindingsInfo bindingsInfo)
    {
        foreach (var button in bindingsInfo.Buttons)
        {
            Buttons = Buttons.Add(button);
        }
    }

    internal void Register(Keys[] keys)
    {
        foreach (var key in keys)
        {
            Buttons = Buttons.Add(new InputButton(key));
        }
    }

    internal void Register(Buttons[] buttons)
    {
        foreach (var button in buttons)
        {
            Buttons = Buttons.Add(new InputButton(button));
        }
    }

    internal void Register(MouseButtons[] buttons)
    {
        foreach (var button in buttons)
        {
            Buttons = Buttons.Add(new InputButton(button));
        }
    }
    internal void Register(GamepadAxis[] buttons)
    {
        foreach (var button in buttons)
        {
            Buttons = Buttons.Add(new InputButton(button));
        }
    }

    public InputButton LastPressedButton(bool keyboard)
    {
        if (_lastPressedButton[keyboard ? 1 : 0] is InputButton button)
        {
            return button;
        }
        foreach (var b in Buttons)
        {
            if (b.Source == InputSource.Gamepad && keyboard)
                continue;

            if ((b.Source == InputSource.Mouse || b.Source == InputSource.Keyboard) && !keyboard)
                continue;

            return b;
        }

        return Buttons.FirstOrDefault();
    }

}