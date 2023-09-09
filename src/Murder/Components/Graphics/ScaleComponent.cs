using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;

namespace Murder.Components.Graphics;

public readonly record struct ScaleComponent(
    [property: ShowInEditor] Vector2 Scale
) : IComponent;