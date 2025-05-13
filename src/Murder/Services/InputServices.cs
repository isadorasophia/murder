using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Assets.Input;
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
using static Murder.Assets.InputGraphicsAsset;

namespace Murder.Services;

public static class InputServices
{
    /// <summary>  
    /// Creates a menu info object for input bindings, allowing customization of player controls.  
    /// </summary>  
    /// <param name="reset">Optional text for an option to reset all bidings to default ones</param>
    /// <param name="exitText">Optional text for the exit option in the menu.</param>  
    /// <returns>A <see cref="GenericMenuInfo{InputMenuOption}"/> containing the input bindings menu options.</returns>  
    public static GenericMenuInfo<InputMenuOption> CreateBindingsMenuInfo(string? reset, string? exitText)
    {
        var builder = ImmutableArray.CreateBuilder<InputMenuOption>();

        if (Game.Data.TryGetAsset<InputGraphicsAsset>(Game.Profile.InputGraphics) is not InputGraphicsAsset inputGraphics)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return new GenericMenuInfo<InputMenuOption>();
        }

        if (Game.Data.TryGetAsset<InputProfileAsset>(Game.Profile.InputProfile) is not InputProfileAsset profile)
        {
            GameLogger.Error("Input profile not set in the GameProfile asset!");
            return new GenericMenuInfo<InputMenuOption>();
        }

        foreach (var buttonInfo in profile.Buttons)
        {
            if (!buttonInfo.AllowPlayerCustomization)
            {
                continue;
            }

            var virtualButton = Game.Input.GetOrCreateButton(buttonInfo.ButtonId);

            string text = $"{buttonInfo.LocalizedName}";

            builder.Add(new InputMenuOption(text, InputMenuOption.InputStyle.Button, buttonInfo.ButtonId));
        }

        foreach (var axisInfo in profile.Axis)
        {
            if (!axisInfo.AllowPlayerCustomization)
            {
                continue;
            }
            var virtualAxis = Game.Input.GetOrCreateAxis(axisInfo.AxisId);
            string text = $"{axisInfo.LocalizedName}";

            if (axisInfo.Horizontal && axisInfo.Vertical)
            {
                builder.Add(new InputMenuOption(text + " (analogue)", InputMenuOption.InputStyle.AxisAnalogue, axisInfo.AxisId));
            }


            if (axisInfo.Vertical)
            {
                builder.Add(new InputMenuOption(text + " (up)", InputMenuOption.InputStyle.AxisDigitalUp, axisInfo.AxisId));
            }
            if (axisInfo.Horizontal)
            {
                builder.Add(new InputMenuOption(text + " (left)", InputMenuOption.InputStyle.AxisDigitalLeft, axisInfo.AxisId));
            }
            if (axisInfo.Vertical)
            {
                builder.Add(new InputMenuOption(text + " (down)", InputMenuOption.InputStyle.AxisDigitalDown, axisInfo.AxisId));
            }
            if (axisInfo.Horizontal)
            {
                builder.Add(new InputMenuOption(text + " (right)", InputMenuOption.InputStyle.AxisDigitalRight, axisInfo.AxisId));
            }
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
        if (Game.Data.TryGetAsset<InputGraphicsAsset>(Game.Profile.InputGraphics) is not InputGraphicsAsset inputGraphicsAsset)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return (null, null);
        }

        // TODO: Detect changes in the asset and rebuild the cache if needed
        if (_graphicsCacheKeys == null)
        {
            InputServices.BuildGraphicsCache(inputGraphicsAsset);
        }

        switch (key.Source)
        {
            case InputSource.Keyboard:
                if (key.Keyboard is Keys keyCode && _graphicsCacheKeys!.TryGetValue(keyCode, out var graphicsIndex))
                {
                    var graphics = inputGraphicsAsset.Graphics[graphicsIndex];
                    return (graphics.Icon.HasImage ? graphics.Icon : inputGraphicsAsset.KeyboardDefault, graphics.Text);
                }
                else
                {
                    return (inputGraphicsAsset.KeyboardDefault, key.Keyboard.ToString());
                }

            case InputSource.Mouse:
                if (key.Mouse is MouseButtons mouseButton && _graphicsCacheMouseButtons!.TryGetValue(mouseButton, out var mouseGraphicsIndex))
                {
                    var mouseGraphics = inputGraphicsAsset.Graphics[mouseGraphicsIndex];
                    return (mouseGraphics.Icon.HasValue ? mouseGraphics.Icon : inputGraphicsAsset.MouseDefault, mouseGraphics.Text);
                }
                else
                {
                    return (inputGraphicsAsset.MouseDefault, key.Mouse.ToString());
                }
            case InputSource.Gamepad:
                if (key.Gamepad is Buttons button && _graphicsCacheButtons!.TryGetValue(button, out var buttonGraphicsIndex))
                {
                    var buttonGraphics = inputGraphicsAsset.Graphics[buttonGraphicsIndex];
                    return (buttonGraphics.Icon.HasImage ? buttonGraphics.Icon : inputGraphicsAsset.GamepadDefault, buttonGraphics.Text);
                }
                else
                {
                    return (inputGraphicsAsset.GamepadDefault, key.Gamepad.ToString());
                }
            case InputSource.GamepadAxis:
                if (key.Axis is GamepadAxis axis && _graphicsCacheGamepadAxis!.TryGetValue(axis, out var axisGraphicsIndex))
                {
                    var axisGraphics = inputGraphicsAsset.Graphics[axisGraphicsIndex];
                    return (axisGraphics.Icon.HasValue ? axisGraphics.Icon : inputGraphicsAsset.GamepadAxisDefault, axisGraphics.Text);
                }
                else
                {
                    return (inputGraphicsAsset.GamepadAxisDefault, key.Axis.ToString());
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

    private static void BuildGraphicsCache(InputGraphicsAsset asset)
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
                        if (keysBuilder.ContainsKey(key))
                        {
                            GameLogger.Warning($"{key} added twice!");
                        }

                        keysBuilder[key] = i;
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a keyboard key for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Mouse:
                    if (graphics.InputButton.Mouse is MouseButtons mouseButton)
                    {
                        if (mouseButtonsBuilder.ContainsKey(mouseButton))
                        {
                            GameLogger.Warning($"{mouseButton} added twice!");
                        }

                        mouseButtonsBuilder[mouseButton] = i;
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a mouse button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.Gamepad:
                    if (graphics.InputButton.Gamepad is Buttons button)
                    {
                        if (buttonsBuilder.ContainsKey(button))
                        {
                            GameLogger.Warning($"{button} added twice!");
                        }

                        buttonsBuilder[button] = i;
                    }
                    else
                    {
                        GameLogger.Error($"Input source/button mismatch is not a gamepad button for {graphics.InputButton}");
                    }
                    break;
                case InputSource.GamepadAxis:
                    if (graphics.InputButton.Axis is GamepadAxis axis)
                    {
                        if (gamepadAxisBuilder.ContainsKey(axis))
                        {
                            GameLogger.Warning($"{axis} added twice!");
                        }

                        gamepadAxisBuilder[axis] = i;
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
        if (Game.Data.TryGetAsset<InputProfileAsset>(Game.Profile.InputProfile) is not InputProfileAsset inputProfile)
        {
            GameLogger.Error("Input information not set in the GameProfile asset!");
            return null;
        }

        // TODO: This should be cached
        foreach (var buttonInfo in inputProfile.Buttons)
        {
            if (buttonInfo.ButtonId == id)
            {
                return buttonInfo;
            }
        }

        return null;
    }

}
