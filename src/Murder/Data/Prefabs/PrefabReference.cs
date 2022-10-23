using Murder.Assets;

namespace Murder.Prefabs
{
    /// <summary>
    /// Represents an entity placed on the map.
    /// </summary>
    public struct PrefabReference
    {
        /// <summary>
        /// Reference to a <see cref="PrefabAsset"/>.
        /// </summary>
        public readonly Guid Guid;

        public PrefabReference(Guid guid) => Guid = guid;

        public PrefabAsset Fetch()
        {
            return Game.Data.GetAsset<PrefabAsset>(Guid);
        }

        public bool CanFetch => Game.Data.HasAsset<PrefabAsset>(Guid);
    }
}
