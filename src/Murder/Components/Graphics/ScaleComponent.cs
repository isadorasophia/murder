using Bang.Components;
using Murder.Attributes;
using System.Numerics;

namespace Murder.Components.Graphics;

public readonly record struct ScaleComponent(
    [property: ShowInEditor] Vector2 Scale
) : IComponent;