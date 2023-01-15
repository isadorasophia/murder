using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets
{
    public abstract class SoundAsset : GameAsset
    {
        public override char Icon => '\uf7a6';
        public override string EditorFolder => "#\uf7a6Sounds";
        public override Vector4 EditorColor => new Vector4(0.5f, 1, 0.2f, 1);
        
        public abstract string Sound();
    }
}