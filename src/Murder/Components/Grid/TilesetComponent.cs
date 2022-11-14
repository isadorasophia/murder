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
        public readonly ImmutableArray<Guid> Tilesets = ImmutableArray<Guid>.Empty;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly ImmutableArray<Guid> Floors = ImmutableArray<Guid>.Empty;

        public TilesetComponent() { }

        public TilesetComponent(ImmutableArray<Guid> tilesets, ImmutableArray<Guid> floors)
        {
            Tilesets = tilesets;
            Floors = floors;
        }
    }
}