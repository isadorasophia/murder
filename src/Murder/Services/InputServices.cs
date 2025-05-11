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
using static Murder.Assets.InputInformationAsset;

namespace Murder.Services;

public static class InputServices
{
    /// <summary>  
    /// Creates a menu info object for input bindings, allowing customization of player controls.  
    /// </summary>  
    /// <param name="exitText">Optional text for the exit option in the menu.</param>  
    /// <returns>A <see cref="GenericMenuInfo{InputMenuOption}"/> containing the input bindings menu options.</returns>  
    public static GenericMenuInfo<InputMenuOption> CreateBindingsMenuInfo(string? exitText)
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

        if (exitText != null)
        {
            builder.Add(new InputMenuOption(exitText, InputMenuOption.InputStyle.None, null));
        }

        return new GenericMenuInfo<InputMenuOption>(builder.ToArray());
    }


    public static DrawMenuInfo DrawBindingsMenu(Batch2D batch,
        in Point position,
        in DrawMenuStyle style,
        in GenericMenuInfo<InputMenuOption> menuInfo,
        in int maxItemsPerLine,
        int totalMenuWidth,
        float sort = .1f)
    {
        PixelFont font = Game.Data.GetFont(style.Font);
        int lineHeight = font.LineHeight + style.ExtraVerticalSpace + 11;

        Point finalPosition = new(Math.Max(position.X, 0), Math.Max(position.Y, 0));
        Point textFinalPosition = new(Math.Max(position.X, 0), Math.Max(position.Y, 0));

        int collumns = 3;// Math.Max(Calculator.CeilToInt(menuInfo.Length / (float)maxItemsPerLine), 1);
        int itemMaxWidth = (int)(totalMenuWidth / collumns);

        Vector2 CalculateText(int line, int column) => new Point(
            Calculator.RoundToInt(column * itemMaxWidth - (totalMenuWidth / 2f)),
            Calculator.FloorToInt(lineHeight * (line + 1.25f))) + textFinalPosition;

        Vector2 CalculateSelector(int line, int column) => new Point(
            (column + 0.5f) * itemMaxWidth - (totalMenuWidth / 2f) - 4,
            lineHeight * (line + 1f) - 4) + finalPosition;

        var infoBindings = Game.Data.GetAsset<InputInformationAsset>(Game.Profile.InputInformation);

        for (int i = 0; i < menuInfo.Length; i++)
        {
            int column = Calculator.FloorToInt(i / (float)maxItemsPerLine);
            int line = i % maxItemsPerLine;
            int x = 0;

            var label = menuInfo.Options[i].Text;
            Vector2 labelPosition = CalculateText(line, column);

            Color currentColor;
            Color? currentShadow;
            currentColor = i == menuInfo.Selection ? style.SelectedColor : style.Color;
            currentShadow = style.Shadow;

            Point textSize = RenderServices.DrawText(batch, style.Font, label ?? string.Empty, labelPosition, itemMaxWidth, new DrawInfo(sort)
            {
                Origin = new Vector2(0, 0.5f),
                Color = currentColor,
                Shadow = currentShadow,
                Debug = false
            });

            x += textSize.X + 16;

            if (infoBindings == null)
            {
                GameLogger.Error("Input information not set in the GameProfile asset!");
                continue;
            }

            if (menuInfo.Options[i].Id is not int buttonId)
            {
                continue;
            }

            if (menuInfo.Options[i].Style == InputMenuOption.InputStyle.Axis)
            {
                var axis = Game.Input.GetOrCreateAxis(buttonId);
                if (axis == null)
                {
                    continue;
                }
                // Draw each individual icon
                foreach (var inputButton in axis.ButtonAxis)
                {
                    // Axis buttons are not implemented yet
                }
            }
            else if (Game.Input.GetOrCreateButton(buttonId) is VirtualButton virtualButton)
            {
                // Draw each individual icon
                foreach (var inputButton in virtualButton.Buttons)
                {
                    (Portrait? icon, string? extraText) = InputServices.GetGraphicsFor(inputButton);
                    Vector2 iconPosition = new Vector2(labelPosition.X + x, labelPosition.Y - 4);

                    if (icon != null)
                    {
                        RenderServices.DrawPortrait(batch, icon.Value, iconPosition, new DrawInfo(sort)
                        {
                            Color = currentColor,
                            Outline = currentShadow,
                            Debug = false
                        });
                    }

                    if (extraText != null)
                    {
                        Point extraTextSize = RenderServices.DrawText(batch, style.Font, extraText, iconPosition, itemMaxWidth, new DrawInfo(sort)
                        {
                            Origin = new Vector2(0.5f, 0.5f),
                            Color = style.Color,
                            Shadow = currentShadow,
                            Debug = false
                        });
                    }

                    x += 16;
                }
            }

        }

        // Get the line and collumn of the selector
        int selectorColumn = Calculator.FloorToInt(menuInfo.Selection / (float)maxItemsPerLine);
        int selectorLine = menuInfo.Selection % maxItemsPerLine;

        Vector2 selectorPosition = CalculateSelector(selectorLine, selectorColumn) + new Vector2(0, MathF.Floor(lineHeight / 2f) - 3);

        selectorColumn = Calculator.FloorToInt(menuInfo.PreviousSelection / (float)maxItemsPerLine);
        selectorLine = menuInfo.PreviousSelection % maxItemsPerLine;
        Vector2 previousSelectorPosition = CalculateSelector(selectorLine, selectorColumn) + new Vector2(0, lineHeight / 2f - 2);

        Vector2 easedPosition;
        if (style.SelectorMoveTime == 0)
            easedPosition = selectorPosition;
        else
            easedPosition = Vector2.Lerp(previousSelectorPosition, selectorPosition,
            Ease.Evaluate(Calculator.ClampTime(Game.NowUnscaled - menuInfo.LastMoved, style.SelectorMoveTime), style.Ease));

        return new DrawMenuInfo()
        {
            SelectorPosition = selectorPosition,
            PreviousSelectorPosition = previousSelectorPosition,
            SelectorEasedPosition = easedPosition.Point(),
            MaximumSelectionWidth = Calculator.RoundToInt(totalMenuWidth / collumns)
        };
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
                    return (graphics.Icon, graphics.Text);
                }
                else
                {
                    return (inputInformationAsset.KeyboardDefault, key.Keyboard.ToString());
                }

            case InputSource.Mouse:
                if (key.Mouse is MouseButtons mouseButton && _graphicsCacheMouseButtons!.TryGetValue(mouseButton, out var mouseGraphicsIndex))
                {
                    var mouseGraphics = inputInformationAsset.Graphics[mouseGraphicsIndex];
                    return (mouseGraphics.Icon, mouseGraphics.Text);
                }
                else
                {
                    return (inputInformationAsset.MouseDefault, key.Mouse.ToString());
                }
            case InputSource.Gamepad:
                if (key.Gamepad is Buttons button && _graphicsCacheButtons!.TryGetValue(button, out var buttonGraphicsIndex))
                {
                    var buttonGraphics = inputInformationAsset.Graphics[buttonGraphicsIndex];
                    return (buttonGraphics.Icon, buttonGraphics.Text);
                }
                else
                {
                    return (inputInformationAsset.GamepadDefault, key.Gamepad.ToString());
                }
            case InputSource.GamepadAxis:
                if (key.Axis is GamepadAxis axis && _graphicsCacheGamepadAxis!.TryGetValue(axis, out var axisGraphicsIndex))
                {
                    var axisGraphics = inputInformationAsset.Graphics[axisGraphicsIndex];
                    return (axisGraphics.Icon, axisGraphics.Text);
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

    private static ImmutableDictionary<int, int>? _buttonsCache = null;
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
