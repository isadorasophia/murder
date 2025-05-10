using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Data.Save;

public class KeyBindings
{
    private ImmutableDictionary<int, int> _bindings = ImmutableDictionary<int, int>.Empty;
}
