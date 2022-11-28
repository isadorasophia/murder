using Murder.Core.Dialogs;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public class SpeakerAsset : GameAsset
    {
        public override char Icon => '\uf2c1';
        public override string EditorFolder => "#\uf518Story\\#\uf2c1Speakers";

        public readonly string SpeakerName = string.Empty;

        public readonly ImmutableDictionary<string, Portrait> Portraits = ImmutableDictionary<string, Portrait>.Empty;
    }
}
