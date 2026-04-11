using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Assets.Input;
using Murder.Core;
using Murder.Core.Input;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Murder.Services;

public static class InputServices
{
    /// <summary>  
    /// Creates a menu info object for input bindings, allowing customization of player controls.  
    /// </summary>  
    /// <param name="reset">Optional text for an option to reset all bidings to default ones</param>
    /// <param name="exitText">Optional text for the exit option in the menu.</param>  
    /// <param name="swapSubmit">Optional text for swap submit/cancel buttons</param>
    /// <param name="allowMouse">Optional text for allowing mouse as submit/cancel</param>
    /// <returns>A <see cref="GenericMenuInfo{InputMenuOption}"/> containing the input bindings menu options.</returns>  
    public static GenericMenuInfo<InputMenuOption> CreateBindingsMenuInfo(string? reset, string? exitText, string? swapSubmit, string? allowMouse)
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
                // Skip for now, since we don't allow customization
                //builder.Add(new InputMenuOption(text + " (analogue)", InputMenuOption.InputStyle.AxisAnalogue, axisInfo.AxisId));
            }

            if (axisInfo.Vertical || axisInfo.Horizontal)
            {
                builder.Add(new InputMenuOption(text, InputMenuOption.InputStyle.AxisDigitalPress, axisInfo.AxisId));
            }
        }

        if (swapSubmit != null)
        {
            builder.Add(new InputMenuOption(swapSubmit, InputMenuOption.InputStyle.SwapSubmitAndCancel, null));
        }

        if (allowMouse != null)
        {
            builder.Add(new InputMenuOption(allowMouse, InputMenuOption.InputStyle.AllowMouseClicks, null));
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

    public static float ParseFloatSafe(string s)
    {
        try
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }
        catch
        {
            GameLogger.Log($"Error while parsing float {s}! Returning 1...");
            return 1;
        }
    }

    public static int ParseIntSafe(string s, NumberStyles style = NumberStyles.Integer)
    {
        try
        {
            return int.Parse(s, style, CultureInfo.InvariantCulture);
        }
        catch
        {
            GameLogger.Log($"Error while parsing int {s}! Returning 1...");
            return 1;
        }
    }
}
