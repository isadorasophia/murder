using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Input;

/// <summary>
/// Struct used for serializing and saving current bindings on buttons
/// </summary>
public readonly struct ButtonBindingsInfo
{
    public readonly int Key;
    public readonly ImmutableArray<InputButton> Buttons = ImmutableArray<InputButton>.Empty;

    private static readonly ImmutableArray<InputButton>.Builder _buttonsBuilder = ImmutableArray.CreateBuilder<InputButton>();

    public ButtonBindingsInfo(int key, VirtualButton virtualButton)
    {
        Key = key;
        
        _buttonsBuilder.Clear();
        foreach (var button in virtualButton.Buttons)
        {
            _buttonsBuilder.Add(button);
        }

        Buttons = _buttonsBuilder.DrainToImmutable();
    }

    public VirtualButton CreateVirtualButton()
    {
        var button = new VirtualButton();
        button.Register(this);
        return button;
    }
}
