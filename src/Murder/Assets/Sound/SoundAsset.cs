using System.Collections.Immutable;
using System.Numerics;
using Murder.Attributes;
using Murder.Utilities;

namespace Murder.Assets
{
    public class SoundAsset : GameAsset
    {
        public override char Icon => '\uf7a6';
        public override string EditorFolder => "#\uf7a6Sounds";
        public override Vector4 EditorColor => new Vector4(0.5f, 1, 0.2f, 1);

        [Sound]
        [ShowInEditor]
        private readonly ImmutableArray<string> _sounds = ImmutableArray.Create<string>();
        
        public string Sound() => RandomExtensions.GetRandom(_sounds, Game.Random);
    }
}