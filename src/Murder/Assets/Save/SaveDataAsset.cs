using Murder.Attributes;
using Murder.Core;
using Murder.Save;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Assets
{
    /// <summary>
    /// Tracks a saved game with all the player status.
    /// </summary>
    public class SaveData : GameAsset
    {
        /// <summary>
        /// This maps
        ///  [World Guid -> Saved World Guid]
        /// that does not belong to a run and should be persisted.
        /// </summary>
        [JsonProperty]
        [ShowInEditor]
        public ImmutableDictionary<Guid, Guid> SavedWorlds { get; private set; } = ImmutableDictionary<Guid, Guid>.Empty;

        /// <summary>
        /// These are all the dynamic assets within the game session.
        /// </summary>
        [JsonProperty]
        [ShowInEditor]
        public Dictionary<Type, Guid> DynamicAssets { get; private set; } = new();

        [JsonProperty]
        public readonly BlackboardTracker BlackboardTracker = new();

        private const string DataDirectoryName = "Data";

        /// <summary>
        /// This is the name used in-game, specified by the user.
        /// </summary>
        public readonly string SaveName = string.Empty;

        /// <summary>
        /// This is save path, used by its assets.
        /// </summary>
        public readonly string SaveDataRelativeDirectoryPath = string.Empty;

        public SaveData() { }

        /// <summary>
        /// Get a world asset to instantiate in the game.
        /// </summary>
        internal SavedWorld? TryLoadLevel(Guid guid)
        {
            SavedWorld? savedWorld = default;
            if (SavedWorlds.TryGetValue(guid, out Guid savedGuid))
            {
                savedWorld = Game.Data.TryGetAssetForCurrentSave(savedGuid) as SavedWorld;
            }

            return savedWorld;
        }

        /// <summary>
        /// Save a world state.
        /// </summary>
        /// <returns>
        /// If there was an existing world instance, it will return the one it replaced.
        /// </returns>
        public virtual void SynchronizeWorld(Guid worldGuid, MonoWorld world)
        {
            SaveWorld(worldGuid, world);
        }

        /// <summary>
        /// This saves a world that should be persisted across several runs.
        /// For now, this will be restricted to the city.
        /// </summary>
        private void SaveWorld(Guid worldGuid, MonoWorld world)
        {
            SavedWorld state = SavedWorld.Create(world);
            Game.Data.AddAssetForCurrentSave(state);

            // Replace and delete the instance it has just replaced.
            SavedWorlds.TryGetValue(worldGuid, out Guid previousState);
            Game.Data.RemoveAssetForCurrentSave(previousState);

            SavedWorlds = SavedWorlds.SetItem(worldGuid, state.Guid);
        }

        public virtual T? TryGetDynamicAsset<T>() where T : DynamicAsset
        {
            T? result = default;

            if (DynamicAssets.TryGetValue(typeof(T), out Guid guid))
            {
                return Game.Data.TryGetAssetForCurrentSave(guid) as T;
            }

            return result;
        }

        internal void SaveDynamicAsset<T>(Guid guid)
        {
            DynamicAssets[typeof(T)] = guid;
        }

        internal void RemoveDynamicAsset(Type t)
        {
            if (DynamicAssets.TryGetValue(t, out Guid value))
            {
                DynamicAssets.Remove(t);

                Game.Data.RemoveAssetForCurrentSave(value);
            }
        }
    }
}