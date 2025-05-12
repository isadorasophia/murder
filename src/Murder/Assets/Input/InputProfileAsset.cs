using Microsoft.Xna.Framework.Input;
using Murder.Attributes;
using System;
using System.Collections.Immutable;
using static Murder.Assets.InputGraphicsAsset;

namespace Murder.Assets.Input;

public class InputProfileAsset : GameAsset
{
    public ImmutableArray<InputInformation> Axis = ImmutableArray<InputInformation>.Empty;
    public ImmutableArray<InputInformation> Buttons = ImmutableArray<InputInformation>.Empty;
}

public readonly struct InputInformation
{
    public readonly int ButtonId;

    public readonly bool AllowPlayerCustomization;
    public readonly LocalizedString LocalizedName;

    [Search]
    public readonly ImmutableArray<Keys> DefaultKeyboard = [];
    [Search]
    public readonly ImmutableArray<Buttons> DefaultGamePadButtons = [];

    public InputInformation()
    {

    }
}