using System.Numerics;
using Murder.Attributes;

namespace Murder.Assets
{
    public class SoundAsset : GameAsset
    {
        public override char Icon => '\uf7a6';
        public override string EditorFolder => "#\uf7a6Sounds";
        public override Vector4 EditorColor => new Vector4(0.5f, 1, 0.2f, 1);

        [Sound]
        [ShowInEditor]
        private readonly string _name = string.Empty;

        public string Sound => _name;
    }
}