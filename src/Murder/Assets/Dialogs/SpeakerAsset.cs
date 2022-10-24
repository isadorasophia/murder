using Murder.Assets.Graphics;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public abstract class SpeakerAsset : GameAsset
    {
        public override char Icon => '\uf2c1';
        public override string EditorFolder => "#\uf518Story\\#\uf2c1Speakers";

        public readonly string SpeakerName = string.Empty;

        [GameAssetId(typeof(SpriteAsset))]
        public readonly ImmutableDictionary<string, Guid> Portraits = ImmutableDictionary<string, Guid>.Empty;
    }
}
