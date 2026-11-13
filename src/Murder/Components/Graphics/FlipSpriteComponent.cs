using Bang.Components;
using Murder.Core.Graphics;
using System.Text.Json.Serialization;

namespace Murder.Components.Graphics;

public readonly struct FlipSpriteComponent : IComponent
{
    public readonly ImageFlip Orientation = ImageFlip.Horizontal;

    [JsonConstructor]
    public FlipSpriteComponent(ImageFlip orientation)
    {
        Orientation = orientation;
    }
}
