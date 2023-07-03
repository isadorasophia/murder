using Microsoft.Xna.Framework.Input;
using Murder.Utilities;

namespace Murder.Core.Input;

public class VirtualButton : IVirtualInput
{
    public List<InputButton> Buttons = new List<InputButton>();

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

        InputButton? pressed = null;
        foreach (var button in Buttons)
        {
            if (button.Check(inputState))
            {
                Down = true;
                pressed = button;
                break;
            }
        }

        if (pressed != null)
        {
            Game.Input.UsingKeyboard = pressed.Value.Source == InputSource.Keyboard;

        }
        
        if (!Down)
        {
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

    internal void Consume()
    {
        Consumed = true;
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

    internal void Register(Keys[] keys)
    {
        foreach (var key in keys)
        {
            Buttons.Add(new InputButton(key));
        }
    }
    
    internal void Register(Buttons[] buttons)
    {
        foreach (var button in buttons)
        {
            Buttons.Add(new InputButton(button));
        }
    }

    internal void Register(MouseButtons[] buttons)
    {
        foreach (var button in buttons)
        {
            Buttons.Add(new InputButton(button));
        }
    }
}
