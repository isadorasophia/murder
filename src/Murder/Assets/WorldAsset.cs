using Newtonsoft.Json;
using Bang;
using Bang.Systems;
using System.Collections.Immutable;
using System.Numerics;
using Murder.Core.Graphics;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Serialization;

namespace Murder.Assets
{
    public class WorldAsset : GameAsset, IWorldAsset
    {
        public override char Icon => '\uf279';
        public override string EditorFolder => "#\uf57dWorld";
        public override Vector4 EditorColor => new Vector4(0.3f, 0.6f, 0.9f, 1);
        public override string SaveLocation => Path.Join(Game.Profile.ContentECSPath, FileHelper.Clean(EditorFolder));

        /// <summary>
        /// Map of all the systems and whether they are active or not.
        /// </summary>
        [JsonProperty]
        private ImmutableArray<(Type systemType, bool isActive)> _systems = ImmutableArray<(Type systemType, bool isActive)>.Empty;
        [JsonProperty]
        private ImmutableArray<(Guid guid, bool isActive)> _features = ImmutableArray<(Guid guid, bool isActive)>.Empty;

        /// <summary>
        /// These are the collection of entities grouped within a folder, distinguished by name.
        /// </summary>
        [JsonProperty]
        private readonly Dictionary<string, ImmutableArray<Guid>> _folders = new();

        private ImmutableArray<Guid>? _instancesCache = null;

        [JsonProperty]
        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public ImmutableArray<(Type systemType, bool isActive)> Systems => _systems;

        public ImmutableArray<(Guid guid, bool isActive)> Features => _features;

        public ImmutableArray<Guid> Instances => _instancesCache ??= _entities.Keys.ToImmutableArray();

        /// <summary>
        /// This is for editor purposes, we group all entities in "folders" when visualizing them.
        /// This has no effect in the actual game.
        /// </summary>
        public ImmutableDictionary<string, ImmutableArray<Guid>> FetchFolders() => _folders.ToImmutableDictionary();

        /// <summary>
        /// Track each group that an entity belongs. Used for speeding up removing entities
        /// and moving them around.
        /// </summary>
        [JsonProperty]
        private readonly Dictionary<Guid, string> _entitiesToFolder = new();

        public bool HasSystems
        {
            get
            {
                if (Systems.Count() > 0)
                    return true;

                foreach (var feature in Features)
                {
                    if (!feature.isActive)
                        continue;

                    if (Game.Data.GetAsset<FeatureAsset>(feature.guid).HasSystems)
                        return true;
                }

                return false;
            }
        }

        internal ImmutableArray<EntityInstance> FetchInstances() => _entities.Values.ToImmutableArray();

        public MonoWorld CreateInstance(Camera2D camera) => CreateInstance(camera, FetchInstances());

        public MonoWorld CreateInstanceFromSave(SavedWorld savedInstance, Camera2D camera) => CreateInstance(camera, savedInstance.FetchInstances());

        private MonoWorld CreateInstance(Camera2D camera, ImmutableArray<EntityInstance> instances)
        {
            List<(ISystem, bool)> systems = new();

            // Actually instantiate and add each of our system types.
            var allSystemTypes = FetchAllSystems();
            foreach (var (type, isActive) in allSystemTypes)
            {
                if (Activator.CreateInstance(type) is ISystem system)
                {
                    systems.Add((system, isActive));
                }
                else
                {
                    GameLogger.Error($"The {type} is not a valid system!");
                }
            }

            MonoWorld world = new(systems, camera, worldAssetGuid: Guid);
            CreateAllEntities(world, instances);

            return world;
        }

        public ImmutableArray<(Type system, bool isActive)> FetchAllSystems()
        {
            var systems = ImmutableArray.CreateBuilder<(Type, bool)>();

            // First, let's add our own systems - easy!
            systems.AddRange(_systems);

            // Now, let's fetch each of our features...
            foreach (var feature in _features)
            {
                FeatureAsset featureAsset = Game.Data.GetAsset<FeatureAsset>(feature.guid);
                systems.AddRange(featureAsset.FetchAllSystems(feature.isActive));
            }

            return systems.ToImmutable();
        }

        /// <summary>
        /// Create all entities into the world.
        /// </summary>
        /// <returns>
        /// Id of all the created entities.
        /// </returns>
        private static void CreateAllEntities(World world, ImmutableArray<EntityInstance> instances)
        {
            foreach (EntityInstance e in instances)
            {
                IWorldAsset.TryCreateEntityInWorld(world, e);
            }
        }

        /// <summary>
        /// Validate instances are remove any entities that no longer exist in the asset.
        /// </summary>
        public void ValidateInstances()
        {
            foreach (EntityInstance e in FetchInstances())
            {
                if (e is PrefabEntityInstance prefabInstance &&
                    !prefabInstance.PrefabRef.CanFetch)
                {
                    // Entity no longer exists, it was probably removed.
                    _entities.Remove(e.Guid);
                    _instancesCache = null;
                }
            }
        }

        public void AddInstance(EntityInstance e)
        {
            _entities.Add(e.Guid, e);

            _instancesCache = null;
        }

        public EntityInstance? TryGetInstance(Guid instanceGuid)
        {
            if (_entities.TryGetValue(instanceGuid, out var entity))
            {
                return entity;
            }

            return null;
        }

        public void RemoveInstance(Guid instanceGuid)
        {
            _entities.Remove(instanceGuid);
            _instancesCache = null;

            // If entity belong to a group, also remove it there.
            if (_entitiesToFolder.TryGetValue(instanceGuid, out string? group)
                && _folders.TryGetValue(group, out ImmutableArray<Guid> instances))
            {
                int index = instances.IndexOf(instanceGuid);
                if (index != -1)
                {
                    _folders[group] = instances.RemoveAt(index);
                }
            }
        }

        public void UpdateSystems(ImmutableArray<(Type systemType, bool isActive)> systems) => _systems = systems;
        public void UpdateFeatures(ImmutableArray<(Guid guid, bool isActive)> features) => _features = features;

        /// <summary>
        /// Add a new folder to group entities.
        /// </summary>
        public bool AddGroup(string name)
        {
            if (_folders.ContainsKey(name))
            {
                return false;
            }

            _folders[name] = ImmutableArray<Guid>.Empty;
            return true;
        }

        public bool HasGroup(string name) => _folders.ContainsKey(name);

        /// <summary>
        /// Delete a new folder to group entities.
        /// </summary>
        public bool DeleteGroup(string name)
        {
            if (!_folders.ContainsKey(name))
            {
                return false;
            }

            _folders.Remove(name);
            return true;
        }

        /// <summary>
        /// Rename a group of entities.
        /// </summary>
        public bool RenameGroup(string previousName, string newName)
        {
            if (!_folders.ContainsKey(previousName))
            {
                return false;
            }

            _folders[newName] = _folders[previousName];
            _folders.Remove(previousName);

            return true;
        }

        public bool MoveToGroup(string? targetGroup, Guid instance, int targetPosition = 0)
        {
            ImmutableArray<Guid> instances;

            // First, remove from any prior group, if it belong to one.
            if (_entitiesToFolder.TryGetValue(instance, out string? fromGroup) &&
                _folders.TryGetValue(fromGroup, out instances))
            {
                _folders[fromGroup] = instances.Remove(instance);
            }
            
            if (targetGroup is null)
            {
                _entitiesToFolder.Remove(instance);
            }
            else
            {
                // Now, move to the target group.
                if (!_folders.TryGetValue(targetGroup, out instances))
                {
                    // Target folder was not found?
                    return false;
                }
                
                _folders[targetGroup] = instances.Insert(targetPosition, instance);
                _entitiesToFolder[instance] = targetGroup;
            }

            return true;
        }

        /// <summary>
        /// Checks whether an entity belongs to any group.
        /// </summary>
        public bool BelongsToAnyGroup(Guid entity) => _entitiesToFolder.ContainsKey(entity);

        /// <summary>
        /// Returns the group that an entity belongs.
        /// </summary>
        public string? GetGroupOf(Guid entity)
        {
            if (_entitiesToFolder.TryGetValue(entity, out string? group))
            {
                return group;
            }

            return null;
        }

        public ImmutableArray<Guid> FetchEntitiesOfGroup(string name) => _folders[name];

        public int GroupsCount() => _folders.Count;
    }
}
