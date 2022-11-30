using Bang.Components;
using Murder.Assets;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Component for an entity that is able to speak with other elements.
    /// </summary>
    public readonly struct SpeakerComponent : IComponent
    {
        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid Speaker;

        public SpeakerComponent(Guid speaker) => Speaker = speaker;
    }
}
