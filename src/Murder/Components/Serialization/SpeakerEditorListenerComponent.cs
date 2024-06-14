using Bang.Components;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [SoundPlayer]
    public readonly struct SpeakerEditorListenerComponent : IComponent
    {
        [Tooltip("Speaker to listen to the events")]
        public readonly AssetRef<SpeakerAsset> Speaker = new();

        public SpeakerEditorListenerComponent() { }
    }
}