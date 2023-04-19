using Murder.Attributes;
using Murder.Core.Geometry;
using Newtonsoft.Json;

namespace Murder.Core.Particles
{
    public readonly struct ParticleTexture
    {
        public readonly ParticleTextureKind Kind = ParticleTextureKind.Point;

        [Tooltip("Asset which will be used to display the texture.")]
        public readonly Guid Asset = Guid.Empty;
        public readonly string Texture = string.Empty;

        public readonly Rectangle Rectangle = Rectangle.One;
        public readonly Circle Circle = new(1);

        [JsonConstructor]
        public ParticleTexture() { }
        
        public ParticleTexture(Rectangle rectangle)
        {
            Kind = ParticleTextureKind.Rectangle;
            Rectangle = rectangle;
        }

        public ParticleTexture(Circle circle)
        {
            Kind = ParticleTextureKind.Circle;
            Circle = circle;
        }

        public ParticleTexture(Guid asset)
        {
            Kind = ParticleTextureKind.Asset;
            Asset = asset;
        }
        public ParticleTexture(string texture)
        {
            Kind = ParticleTextureKind.Texture;
            Texture = texture;
        }
    }
}
