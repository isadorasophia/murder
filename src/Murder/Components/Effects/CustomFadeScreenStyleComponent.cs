using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bang.StateMachines;

namespace Murder.Components;

[Unique]
public readonly struct CustomFadeScreenStyleComponent: IComponent
{
    public readonly string? CustomFadeImage = null;

    public CustomFadeScreenStyleComponent()
    {
        
    }
}
