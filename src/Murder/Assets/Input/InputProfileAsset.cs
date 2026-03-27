using Microsoft.Xna.Framework.Input;
using Murder.Attributes;
using Murder.Core.Input;
using System.Collections.Immutable;

namespace Murder.Assets.Input;

public class InputProfileAsset : GameAsset
{
    public ImmutableArray<InputAxisInformation> Axis = ImmutableArray<InputAxisInformation>.Empty;
    public ImmutableArray<InputInformation> Buttons = ImmutableArray<InputInformation>.Empty;
}
public enum KeyboardTemplate
{
    QWERTY = 0,
    AZERTY = 1,
    Colemak = 2,
    Dvorak = 3,
}

public readonly struct InputInformation
{
    public readonly int ButtonId;

    public readonly bool AllowPlayerCustomization;
    public readonly LocalizedString LocalizedName;

    [Search]
    public readonly ImmutableArray<Keys> DefaultKeyboard = [];

    [Search]
    public readonly ImmutableArray<Keys> DefaultKeyboardMac = [];

    [Search]
    public readonly ImmutableArray<Buttons> DefaultGamePadButtons = [];
    ///[Search]
    public readonly ImmutableArray<MouseButtons> DefaultMouseButtons = [];

    public readonly ImmutableArray<InputButtonAxis> DefaultAxes = [];

    public InputInformation()
    {

    }
}

public readonly struct InputAxisInformation
{
    public readonly int AxisId;

    public readonly bool AllowPlayerCustomization;

    public readonly bool Horizontal = true;
    public readonly bool Vertical = true;

    public readonly LocalizedString LocalizedName;

    public readonly ImmutableArray<GamepadAxis> Analogue = [];
    public readonly ImmutableArray<ImmutableArray<InputButton>> Digital = [];

    public InputAxisInformation()
    {

    }
}