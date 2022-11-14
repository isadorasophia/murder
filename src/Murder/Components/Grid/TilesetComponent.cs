using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using System.Collections.Immutable;

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
        // public readonly ImmutableArray<Guid> Tileset;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Floor;
        // public readonly ImmutableArray<Guid> Floor;

        //public TilesetComponent(ImmutableArray<Guid> tileset, ImmutableArray<Guid> floor)
        //{
        //    Tileset = tileset;
        //    Floor = floor;
        //}
        public TilesetComponent(Guid tileset, Guid floor)
        {
            Tileset = tileset;
            Floor = floor;
        }
    }
}