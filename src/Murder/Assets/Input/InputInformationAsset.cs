using Microsoft.Xna.Framework.Input;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class InputInformationAsset : GameAsset
{
    public override char Icon => '';
    public override string EditorFolder => "#Ui";
    public override Vector4 EditorColor => "#f4eb6f".ToVector4Color();

    public ImmutableArray<InputInformation> Axis = ImmutableArray<InputInformation>.Empty;
    public ImmutableArray<InputInformation> Buttons = ImmutableArray<InputInformation>.Empty;
    public ImmutableArray<ButtonGraphics> Graphics = ImmutableArray<ButtonGraphics>.Empty;

    private ImmutableDictionary<int, InputInformation>? _buttonsCache = null;
    private ImmutableDictionary<Keys, ButtonGraphics>? _graphicsCacheKeys = null;
    private ImmutableDictionary<Buttons, ButtonGraphics>? _graphicsCacheButtons = null;
    private ImmutableDictionary<MouseButtons, ButtonGraphics>? _graphicsCacheMouseButtons = null;
    private ImmutableDictionary<GamepadAxis, ButtonGraphics>? _graphicsCacheGamepadAxis = null;

    public InputInformation? GetButtonInfo(int id)
    {
        if (_buttonsCache == null)
        {
            _buttonsCache = BuildButtonsCache();
        }

        if (_buttonsCache.TryGetValue(id, out InputInformation buttonInfo))
        {
            return buttonInfo;
        }

        return null;
    }

    private ImmutableDictionary<int, InputInformation> BuildButtonsCache()
    {
        var builder = ImmutableDictionary.CreateBuilder<int, InputInformation>();
        foreach (var info in Buttons)
        {
            builder.Add(info.ButtonId, info);
        }

        return builder.ToImmutable();
    }

    public (Portrait?, string?) GetGraphicsFor(InputButton key)
    {
        if (_graphicsCacheKeys == null)
        {
            BuildGraphicsCache();
        }

        switch (key.Source)
        {
            case InputSource.Keyboard:
                if (key.Keyboard is Keys keyCode && _graphicsCacheKeys!.TryGetValue(keyCode, out var graphics))
                {
                    return (graphics.Icon, graphics.Text);
                }
                return (null, null);
            case InputSource.Mouse:
                if (key.Mouse is MouseButtons mouseButton && _graphicsCacheMouseButtons!.TryGetValue(mouseButton, out var mouseGraphics))
                {
                    return (mouseGraphics.Icon, mouseGraphics.Text);
                }
                return (null, null);
            case InputSource.Gamepad:
                if (key.Gamepad is Buttons button && _graphicsCacheButtons!.TryGetValue(button, out var buttonGraphics))
                {
                    return (buttonGraphics.Icon, buttonGraphics.Text);
                }
                return (null, null);
            case InputSource.GamepadAxis:
                if (key.Axis is GamepadAxis axis && _graphicsCacheGamepadAxis!.TryGetValue(axis, out var axisGraphics))
                {
                    return (axisGraphics.Icon, axisGraphics.Text);
                }
                return (null, null);
            default:
                return (null, null);
        }
    }

    private void BuildGraphicsCache()
    {
        var keysBuilder = ImmutableDictionary.CreateBuilder<Keys, ButtonGraphics>();
        var buttonsBuilder = ImmutableDictionary.CreateBuilder<Buttons, ButtonGraphics>();
        var mouseButtonsBuilder = ImmutableDictionary.CreateBuilder<MouseButtons, ButtonGraphics>();
        var gamepadAxisBuilder = ImmutableDictionary.CreateBuilder<GamepadAxis, ButtonGraphics>();

        foreach (var graphics in Graphics)
        {
            switch (graphics.InputButton.Source)
            {
                case InputSource.None:
                    GameLogger.Error($"Input button source is None for {graphics.InputButton}");
                    break;
                case InputSource.Keyboard:
                    if (graphics.InputButton.Keyboard is Keys key)
                    {
                        keysBuilder.Add(key, graphics);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a keyboard key for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Mouse:
                    if (graphics.InputButton.Mouse is MouseButtons mouseButton)
                    {
                        mouseButtonsBuilder.Add(mouseButton, graphics);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a mouse button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Gamepad:
                    if (graphics.InputButton.Gamepad is Buttons button)
                    {
                        buttonsBuilder.Add(button, graphics);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a gamepad button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.GamepadAxis:
                    if (graphics.InputButton.Axis is GamepadAxis axis)
                    {
                        gamepadAxisBuilder.Add(axis, graphics);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a gamepad axis for {graphics.InputButton}");
                    }
                    break;
            }
        }
        _graphicsCacheKeys = keysBuilder.ToImmutable();
        _graphicsCacheButtons = buttonsBuilder.ToImmutable();
        _graphicsCacheMouseButtons = mouseButtonsBuilder.ToImmutable();
        _graphicsCacheGamepadAxis = gamepadAxisBuilder.ToImmutable();
    }

    public readonly struct InputInformation
    {
        public readonly int ButtonId;

        public readonly bool AllowPlayerCustomization;
        public readonly LocalizedString LocalizedName;
    }


    public readonly struct ButtonGraphics
    {
        public readonly Portrait Icon = new();
        public readonly InputButton InputButton = new();
        public readonly string? Text = null;
        public ButtonGraphics()
        {

        }
    }
}
