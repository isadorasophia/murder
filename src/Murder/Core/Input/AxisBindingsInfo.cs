using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Input;

/// <summary>
/// Struct used for serializing and saving current bindings on axes
/// </summary>
public readonly struct AxisBindingsInfo
{
    public readonly int Key;

    public readonly ImmutableArray<InputButtonAxis> Buttons = ImmutableArray<InputButtonAxis>.Empty;

    private static readonly ImmutableArray<InputButtonAxis>.Builder _builder = ImmutableArray.CreateBuilder<InputButtonAxis>();

    public AxisBindingsInfo(int key, VirtualAxis virtualAxis)
    {
        Key = key;

        _builder.Clear();
        foreach (var axis in virtualAxis.ButtonAxis)
        {
            _builder.Add(axis);
        }

        Buttons = _builder.DrainToImmutable();
    }

    public VirtualAxis CreateVirtualAxis()
    {
        var axis = new VirtualAxis();
        axis.Register(this);
        return axis;
    }
}
