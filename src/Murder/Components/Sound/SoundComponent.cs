using Murder.Attributes;
using Bang.Components;
using Murder.Assets;

namespace Murder.Components
{
    /// <summary>
    /// Sound component which will be immediately played and destroyed.
    /// </summary>
    public readonly struct SoundComponent : IComponent
    {
        [GameAssetId(typeof(SoundAsset))]
        public readonly Guid Guid = Guid.Empty;

        public readonly bool DestroyEntity = true;
        public SoundComponent() { }

        public SoundComponent(Guid guid, bool destroyEntity)
        {
            Guid = guid;
            DestroyEntity = destroyEntity;
        }
    }
}
