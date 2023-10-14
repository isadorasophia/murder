﻿using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Editor.Components
{
    /// <summary>
    /// Component for an entity that is currently being placed in the map.
    /// </summary>
    [RuntimeOnly]
    public readonly struct IsPlacingComponent : IComponent { }
}