using System.Collections.Immutable;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Utilities;

namespace Murder.Assets
{
    public class MurderSoundAsset : SoundAsset
    {
        [Sound]
        [ShowInEditor]
        private readonly ImmutableArray<string> _sounds = ImmutableArray<string>.Empty;
        
        public override SoundEventId Sound() => new SoundEventId { Path = RandomExtensions.GetRandom(_sounds, Game.Random) };
    }
}