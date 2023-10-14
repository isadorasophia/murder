using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Particles;

namespace Murder.Components
{
    public readonly struct ParticleSystemComponent : IComponent
    {
        [GameAssetId(typeof(ParticleSystemAsset))]
        public readonly Guid Asset;

        public readonly bool DestroyWhenEmpty;

        public ParticleSystemComponent(Guid asset, bool destroy)
        {
            Asset = asset;
            DestroyWhenEmpty = destroy;
        }
    }
}