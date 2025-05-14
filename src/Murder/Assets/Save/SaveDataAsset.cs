using Bang;
using Bang.Entities;
using Murder.Assets.Save;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Save;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Assets
{
    /// <summary>
    /// Tracks a saved game with all the player status.
    /// </summary>
    public class SaveData : GameAsset
    {
        public override bool StoreInDatabase => false;

        public override bool IsStoredInSaveData => true;
        public override bool IsSavePacked => true;

        /// <summary>
        /// This maps
        ///  [World Guid -> Saved World Guid]
        /// that does not belong to a run and should be persisted.
        /// </summary>
        [Serialize]
        [ShowInEditor]
        public ImmutableDictionary<Guid, Guid> SavedWorlds { get; private set; } = ImmutableDictionary<Guid, Guid>.Empty;

        /// <summary>
        /// List of all consumed entities throughout the map.
        /// Mapped according to:
        /// [World guid -> [Entity Guid]]
        /// </summary>
        [Serialize]
        protected readonly Dictionary<Guid, HashSet<Guid>> _entitiesOnWorldToDestroy = new();

        /// <summary>
        /// This is the last world that the player was by the time this was saved.
        /// </summary>
        [Serialize]
        private Guid? _lastWorld = null;

        public Guid? CurrentWorld => _lastWorld;

        [Serialize]
        public readonly BlackboardTracker BlackboardTracker = null!;

        protected virtual string GetDefaultSaveName() => "Unknown";

        [Serialize]
        private string _saveName = string.Empty;

        /// <summary>
        /// This is the name used in-game, specified by the user.
        /// </summary>
        public string SaveName
        {
            get 
            { 
                return _saveName; 
            }
            private set
            {
                _saveName = value;

                // Also update the save name at the tracker.
                if (Game.Data.GetAllSaves().TryGetValue(SaveSlot, out SaveDataInfo? info))
                {
                    info.Name = value;
                }
            }
        }

        /// <summary>
        /// This is save path, used by its assets.
        /// </summary>
        [Serialize]
        public string SaveRelativeDirectoryPath { get; private set; } = string.Empty;

        /// <summary>
        /// This is save path, used by its assets.
        /// </summary>
        [Serialize]
        public string SaveDataRelativeDirectoryPath { get; private set; } = string.Empty;

        /// <summary>
        /// Which save slot this belongs to. Default is zero.
        /// </summary>
        [Serialize]
        public readonly int SaveSlot = 0;

        /// <summary>
        /// Game version, used for game save compatibility.
        /// </summary>
        public readonly float SaveVersion;

        private const string DataDirectoryName = "Data";

        // Fancy variables for keeping track of async operations.
        public Task? PendingOperation => _pendingOperation;
        private volatile Task? _pendingOperation = null;

        private readonly object _saveLock = new();

        protected SaveData(int saveSlot, float saveVersion, BlackboardTracker blackboardTracker)
        {
            SaveSlot = saveSlot;
            SaveVersion = saveVersion;

            BlackboardTracker = blackboardTracker;

            SaveRelativeDirectoryPath = $"{SaveSlot}";

            FilePath = Path.Join(SaveRelativeDirectoryPath, $"{Name}.json");
            SaveDataRelativeDirectoryPath = Path.Join(SaveRelativeDirectoryPath, DataDirectoryName);
        }

        public SaveData(int saveSlot, float saveVersion) : this(saveSlot, saveVersion, new BlackboardTracker()) { }

        /// <summary>
        /// Called after creating a fresh new save from this.
        /// </summary>
        public void Initialize()
        {
            Guid = Guid.NewGuid();
            Name = Guid.ToString();

            ChangeSaveName("Unknown");
        }

        public void ChangeSaveName(string name)
        {
            SaveName = name;
        }

        public virtual void OnBeforeSave() { }

        /// <summary>
        /// Get a world asset to instantiate in the game.
        /// This tracks the <paramref name="guid"/> at <see cref="_lastWorld"/>.
        /// </summary>
        public virtual SavedWorld? TryLoadLevel(Guid guid)
        {
            SavedWorld? savedWorld = default;
            if (SavedWorlds.TryGetValue(guid, out Guid savedGuid))
            {
                savedWorld = Game.Data.TryGetAssetForCurrentSave(savedGuid) as SavedWorld;
            }

            return savedWorld;
        }

        public void TrackCurrentWorld(Guid guid)
        {
            _lastWorld = guid;
        }

        /// <summary>
        /// This saves a world that should be persisted across several runs.
        /// For now, this will be restricted to the city.
        /// </summary>
        public void SaveAsync(MonoWorld world)
        {
            Task pendingOperation = _pendingOperation ?? Task.CompletedTask;

            lock (_saveLock)
            {
                ImmutableArray<Entity> entitiesOnSaveRequested = world.GetAllEntities();

                Game.Instance.SetWaitForSaveComplete();

                _pendingOperation = Task.Run(async delegate
                {
                    SavedWorld state = await SavedWorld.CreateAsync(world, entitiesOnSaveRequested);
                    Game.Data.AddAssetForCurrentSave(state);

                    // Replace and delete the instance it has just replaced.
                    SavedWorlds.TryGetValue(world.WorldAssetGuid, out Guid previousState);
                    Game.Data.RemoveAssetForCurrentSave(previousState);

                    SavedWorlds = SavedWorlds.SetItem(world.WorldAssetGuid, state.Guid);
                });
            }
        }

        public bool HasFinishedSaveWorld()
        {
            if (_pendingOperation is null)
            {
                return true;
            }

            return _pendingOperation.IsCompleted;
        }

        /// <summary>
        /// This will clean all saved worlds.
        /// </summary>
        protected virtual void ClearAllWorlds()
        {
            foreach (Guid guid in SavedWorlds.Values)
            {
                Game.Data.RemoveAssetForCurrentSave(guid);
            }

            SavedWorlds = ImmutableDictionary<Guid, Guid>.Empty;
        }

        /// <summary>
        /// This records that an entity has been removed from the map.
        /// </summary>
        public bool RecordRemovedEntityFromWorld(World world, Entity entity)
        {
            Guid? guid = EntityToGuid(world, entity);
            if (guid is null)
            {
                GameLogger.Error("An error occurred while getting entity.");
                return false;
            }

            HashSet<Guid> allItems = GetOrCreateEntitiesToBeDestroyedAt(world);
            allItems.Add(guid.Value);

            return true;
        }

        /// <summary>
        /// Fetch the collected items at <paramref name="world"/>.
        /// If none, creates a new empty collection and returns that instead.
        /// </summary>
        protected HashSet<Guid> GetOrCreateEntitiesToBeDestroyedAt(World world)
        {
            Guid worldGuid = world.Guid();
            if (!_entitiesOnWorldToDestroy.TryGetValue(worldGuid, out HashSet<Guid>? allItems))
            {
                allItems = new();
                _entitiesOnWorldToDestroy[worldGuid] = allItems;
            }

            return allItems;
        }

        protected Guid? EntityToGuid(World world, Entity e)
        {
            // We keep track of "root" entities.
            Entity target = EntityServices.FindRootEntity(e)!;
            return EntityToGuid(world, target.EntityId);
        }

        protected Guid? EntityToGuid(World world, int id)
        {
            if (world.TryGetUniqueInstanceToEntityLookup() is not InstanceToEntityLookupComponent lookup)
            {
                GameLogger.Warning("How does this world do not have InstanceToEntityLookupComponent setup?");
                return null;
            }

            if (!lookup.EntitiesToInstances.TryGetValue(id, out Guid guid))
            {
                GameLogger.Warning("KeyComponent has to be set on a root entity.");
                return null;
            }

            return guid;
        }
    }
}