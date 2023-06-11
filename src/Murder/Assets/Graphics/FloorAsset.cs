using Murder.Attributes;
using Murder.Core;
using Murder.Utilities;

namespace Murder.Assets.Graphics
{
    public class FloorAsset : GameAsset
    {
        public override char Icon => '\uf84c';
        public override string EditorFolder => "#Tilesets\\#\uf51aFloors";

        public readonly AssetRef<SpriteAsset> Image = new();

        [Tooltip("Properties of this tileset.")]
        [Default("Add properties")]
        public readonly ITileProperties? Properties = null;

        public FloorAsset() { }
    }
}
