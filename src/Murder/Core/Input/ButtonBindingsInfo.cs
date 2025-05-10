using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Input;

public readonly struct ButtonBindingsInfo
{
    public readonly ImmutableArray<InputButton> Buttons = ImmutableArray<InputButton>.Empty;
    public readonly int Key;

    public static readonly ImmutableArray<InputButton>.Builder buttonsBuilder = ImmutableArray.CreateBuilder<InputButton>();

    public ButtonBindingsInfo(int key, VirtualButton virtualButton) : this()
    {
        Key = key;
        
        buttonsBuilder.Clear();
        foreach (var button in virtualButton.Buttons)
        {
            buttonsBuilder.Add(button);
        }

        Buttons = buttonsBuilder.DrainToImmutable();
    }

    public VirtualButton CreateVirtualButton()
    {
        var button = new VirtualButton();
        button.Register(this);
        return button;
    }
}
