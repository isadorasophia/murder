using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Components
{
    [Flags]
    public enum RoomFlags
    {
        None = 0,
        ClampCamera = 1
    }

    /// <summary>
    /// This describes a room component properties.
    /// </summary>
    [Requires(typeof(TileGridComponent))]
    public readonly struct RoomComponent : IComponent
    {
        [GameAssetId(typeof(FloorAsset))]
        public readonly Guid Floor = Guid.Empty;
        public readonly RoomFlags Flags = RoomFlags.None;

        public RoomComponent() { }

        public RoomComponent(Guid floor) { Floor = floor; }
    }
}