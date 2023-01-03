using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct ParticleSystemComponent : IComponent
    {
        [GameAssetId(typeof(ParticleSystemAsset))]
        public readonly Guid Asset;

        public ParticleSystemComponent(Guid asset)
        {
            Asset = asset;
        }
    }
}
