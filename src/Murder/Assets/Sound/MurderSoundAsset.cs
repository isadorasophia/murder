using System.Collections.Immutable;
using System.Numerics;
using Murder.Attributes;
using Murder.Utilities;

namespace Murder.Assets
{
    public class MurderSoundAsset : SoundAsset
    {
        [Sound]
        [ShowInEditor]
        private readonly ImmutableArray<string> _sounds = ImmutableArray<string>.Empty;
        
        public override string Sound() => RandomExtensions.GetRandom(_sounds, Game.Random);
    }
}