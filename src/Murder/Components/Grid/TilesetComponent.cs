using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    public readonly struct TilesetComponent : IComponent
    {
        [GameAssetId(typeof(TilesetAsset))]
        public readonly Guid Tileset;
        
        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Floor;

        public TilesetComponent(Guid tileset, Guid floor)
        {
            Tileset = tileset;
            Floor = floor;
        }
    }
}