using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This describes a room component properties.
    /// </summary>
    [Requires(typeof(TileGridComponent))]
    public struct RoomComponent : IComponent
    {
        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Floor = Guid.Empty;
        
        public RoomComponent() { }
        public RoomComponent(Guid floor) { Floor = floor; }
    }
}
