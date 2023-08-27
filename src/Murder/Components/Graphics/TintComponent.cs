using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components.Graphics;

public readonly record struct TintComponent(
    [property: ShowInEditor] Color TintColor
) : IComponent;