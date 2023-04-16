using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;

namespace Murder.Components
{
    public readonly struct TextureComponent : IComponent
    {
        [AtlasCoordinates]
        public readonly string Texture = "MissingImage";

        [Tooltip("(0,0) is top left and (1,1) is bottom right"), Slider]
        public readonly Vector2 Offset;

        public TextureComponent(string texture, Vector2 offset) => (Texture, Offset) = (texture, offset);
    }
}
