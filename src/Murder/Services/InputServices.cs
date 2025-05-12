using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Services.Info;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using System.Security.Authentication.ExtendedProtection;
using static Murder.Assets.InputInformationAsset;

namespace Murder.Services;

public static class InputServices
{
    /// <summary>  
    /// Creates a menu info object for input bindings, allowing customization of player controls.  
    /// </summary>  
    /// <param name="exitText">Optional text for the exit option in the menu.</param>  
    /// <returns>A <see cref="GenericMenuInfo{InputMenuOption}"/> containing the input bindings menu options.</returns>  
    public static GenericMenuInfo<InputMenuOption> CreateBindingsMenuInfo(string? reset, string? exitText)
    {
        var builder = ImmutableArray.CreateBuilder<InputMenuOption>();

        if (Game.Data.TryGetAsset<InputInformationAsset>(Game.Profile.InputInformation) is not InputInformationAsset inputInformation)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return new GenericMenuInfo<InputMenuOption>();
        }

        foreach (var buttonInfo in inputInformation.Buttons)
        {
            if (!buttonInfo.AllowPlayerCustomization)
            {
                continue;
            }

            var virtualButton = Game.Input.GetOrCreateButton(buttonInfo.ButtonId);

            string text = $"{buttonInfo.LocalizedName}:";

            builder.Add(new InputMenuOption(text, InputMenuOption.InputStyle.Button, buttonInfo.ButtonId));
        }

        foreach (var axisInfo in inputInformation.Axis)
        {
            if (!axisInfo.AllowPlayerCustomization)
            {
                continue;
            }
            var virtualAxis = Game.Input.GetOrCreateAxis(axisInfo.ButtonId);
            string text = $"{axisInfo.LocalizedName}:";
            builder.Add(new InputMenuOption(text, InputMenuOption.InputStyle.Axis, axisInfo.ButtonId));
        }

        if (reset != null)
        {
            builder.Add(new InputMenuOption(reset, InputMenuOption.InputStyle.None, null));
        }

        if (exitText != null)
        {
            builder.Add(new InputMenuOption(exitText, InputMenuOption.InputStyle.None, null));
        }

        return new GenericMenuInfo<InputMenuOption>(builder.ToArray());
    }

    public static (Portrait?, string?) GetGraphicsFor(InputButton key)
    {
        if (Game.Data.TryGetAsset<InputInformationAsset>(Game.Profile.InputInformation) is not InputInformationAsset inputInformationAsset)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return (null, null);
        }

        // TODO: Detect changes in the asset and rebuild the cache if needed
        if (_graphicsCacheKeys == null)
        {
            InputServices.BuildGraphicsCache(inputInformationAsset);
        }

        switch (key.Source)
        {
            case InputSource.Keyboard:
                if (key.Keyboard is Keys keyCode && _graphicsCacheKeys!.TryGetValue(keyCode, out var graphicsIndex))
                {
                    var graphics = inputInformationAsset.Graphics[graphicsIndex];
                    return (graphics.Icon.HasImage ? graphics.Icon : inputInformationAsset.KeyboardDefault, graphics.Text);
                }
                else
                {
                    return (inputInformationAsset.KeyboardDefault, key.Keyboard.ToString());
                }

            case InputSource.Mouse:
                if (key.Mouse is MouseButtons mouseButton && _graphicsCacheMouseButtons!.TryGetValue(mouseButton, out var mouseGraphicsIndex))
                {
                    var mouseGraphics = inputInformationAsset.Graphics[mouseGraphicsIndex];
                    return (mouseGraphics.Icon.HasValue ? mouseGraphics.Icon : inputInformationAsset.MouseDefault, mouseGraphics.Text);
                }
                else
                {
                    return (inputInformationAsset.MouseDefault, key.Mouse.ToString());
                }
            case InputSource.Gamepad:
                if (key.Gamepad is Buttons button && _graphicsCacheButtons!.TryGetValue(button, out var buttonGraphicsIndex))
                {
                    var buttonGraphics = inputInformationAsset.Graphics[buttonGraphicsIndex];
                    return (buttonGraphics.Icon.HasImage ? buttonGraphics.Icon : inputInformationAsset.GamepadDefault, buttonGraphics.Text);
                }
                else
                {
                    return (inputInformationAsset.GamepadDefault, key.Gamepad.ToString());
                }
            case InputSource.GamepadAxis:
                if (key.Axis is GamepadAxis axis && _graphicsCacheGamepadAxis!.TryGetValue(axis, out var axisGraphicsIndex))
                {
                    var axisGraphics = inputInformationAsset.Graphics[axisGraphicsIndex];
                    return (axisGraphics.Icon.HasValue ? axisGraphics.Icon : inputInformationAsset.GamepadAxisDefault, axisGraphics.Text);
                }
                else
                {
                    return (inputInformationAsset.GamepadAxisDefault, key.Axis.ToString());
                }
            default:
                GameLogger.Warning($"Input button source for {key} is invalid");
                return (null, null);
        }
    }

    private static ImmutableDictionary<Keys, int>? _graphicsCacheKeys = null;
    private static ImmutableDictionary<Buttons, int>? _graphicsCacheButtons = null;
    private static ImmutableDictionary<MouseButtons, int>? _graphicsCacheMouseButtons = null;
    private static ImmutableDictionary<GamepadAxis, int>? _graphicsCacheGamepadAxis = null;

    private static void BuildGraphicsCache(InputInformationAsset asset)
    {
        var keysBuilder = ImmutableDictionary.CreateBuilder<Keys, int>();
        var buttonsBuilder = ImmutableDictionary.CreateBuilder<Buttons, int>();
        var mouseButtonsBuilder = ImmutableDictionary.CreateBuilder<MouseButtons, int>();
        var gamepadAxisBuilder = ImmutableDictionary.CreateBuilder<GamepadAxis, int>();

        for (int i = 0; i < asset.Graphics.Length; i++)
        {
            var graphics = asset.Graphics[i];

            switch (graphics.InputButton.Source)
            {
                case InputSource.None:
                    GameLogger.Error($"Input button source is None for {graphics.InputButton}");
                    break;
                case InputSource.Keyboard:
                    if (graphics.InputButton.Keyboard is Keys key)
                    {
                        keysBuilder.Add(key, i);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a keyboard key for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Mouse:
                    if (graphics.InputButton.Mouse is MouseButtons mouseButton)
                    {
                        mouseButtonsBuilder.Add(mouseButton, i);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a mouse button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Gamepad:
                    if (graphics.InputButton.Gamepad is Buttons button)
                    {
                        buttonsBuilder.Add(button, i);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a gamepad button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.GamepadAxis:
                    if (graphics.InputButton.Axis is GamepadAxis axis)
                    {
                        gamepadAxisBuilder.Add(axis, i);
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a gamepad axis for {graphics.InputButton}");
                    }
                    break;
            }
        }

        // Build the cache
        _graphicsCacheKeys = keysBuilder.ToImmutable();
        _graphicsCacheButtons = buttonsBuilder.ToImmutable();
        _graphicsCacheMouseButtons = mouseButtonsBuilder.ToImmutable();
        _graphicsCacheGamepadAxis = gamepadAxisBuilder.ToImmutable();
    }
    public static void ClearCache()
    {
        _graphicsCacheButtons = null;
        _graphicsCacheKeys = null;
        _graphicsCacheMouseButtons = null;
        _graphicsCacheGamepadAxis = null;
    }
    public static InputInformation? GetButtonInfo(int id)
    {
        if (Game.Data.TryGetAsset<InputInformationAsset>(Game.Profile.InputInformation) is not InputInformationAsset inputInformationAsset)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return null;
        }

        // TODO: This should be cached
        foreach (var buttonInfo in inputInformationAsset.Buttons)
        {
            if (buttonInfo.ButtonId == id)
            {
                return buttonInfo;
            }
        }

        return null;
    }

}
