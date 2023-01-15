using Murder.Attributes;
using Bang.Components;
using Murder.Assets;

namespace Murder.Components
{
    /// <summary>
    /// Music component which will be immediately played and destroyed.
    /// </summary>
    public readonly struct MusicComponent : IComponent
    {
        [GameAssetId(typeof(SoundAsset))]
        public readonly Guid Id = Guid.Empty;

        public MusicComponent() { }
    }
}
