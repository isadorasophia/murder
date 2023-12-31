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

        /// <summary>
        /// Will always draw the floor tile, even if a tileset is occluding it.
        /// </summary>
        [Tooltip("Will always draw the floor tile, even if a tileset is occluding it.")]
        public readonly bool AlwaysDraw = false;

        [Tooltip("Properties of this tileset.")]
        [Default("Add properties")]
        public readonly ITileProperties? Properties = null;

        public FloorAsset() { }
    }
}