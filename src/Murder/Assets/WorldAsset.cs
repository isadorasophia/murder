using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Core;
using Murder.Core.Cutscenes;
using Murder.Core.Graphics;
using Murder.Core.MurderActions;
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Serialization;
using Murder.Services;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets
{
    public class WorldAsset : GameAsset, IWorldAsset
    {
        public Guid WorldGuid => Guid;

        public override char Icon => '\uf279';
        public override string EditorFolder => "#\uf57dWorld";
        public override Vector4 EditorColor => new Vector4(0.3f, 0.6f, 0.9f, 1);
        public override string SaveLocation => Path.Join(Game.Profile.ContentECSPath, FileHelper.Clean(EditorFolder));

        /// <summary>
        /// This is the world name used when fetching this world within the game.
        /// </summary>
        public readonly string WorldName = "World";

        /// <summary>
        /// This is the order in which this world will be displayed in game (when selecting a lvel, etc.)
        /// </summary>
        public readonly int Order = 1;

        /// <summary>
        /// Map of all the systems and whether they are active or not.
        /// </summary>
        [Serialize]
        private ImmutableArray<(Type systemType, bool isActive)> _systems = [];

        [Serialize]
        private ImmutableArray<(Guid guid, bool isActive)> _features = [];

        /// <summary>
        /// Systems which are removed by default.
        /// This is provided separetely since we don't have the power to modify systems that are inside a feature.
        /// </summary>
        [Serialize]
        [TypeOf(typeof(ISystem))]
        private ImmutableArray<Type> _systemsToRemove = [];

        /// <summary>
        /// These are the collection of entities grouped within a folder, distinguished by name.
        /// </summary>
        [Serialize]
        private readonly Dictionary<string, ImmutableArray<Guid>> _folders = new();

        /// <summary>
        /// Additional optional filters.
        /// </summary>
        [Serialize]
        private readonly Dictionary<string, ImmutableArray<Guid>> _filterFolders = new();

        private ImmutableArray<Guid>? _instancesCache = null;

        [Serialize]
        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public ImmutableArray<(Type systemType, bool isActive)> Systems => _systems;

        public ImmutableArray<Type> SystemsToRemove => _systemsToRemove;

        public ImmutableArray<(Guid guid, bool isActive)> Features => _features;

        public ImmutableArray<Guid> Instances => _instancesCache ??= _entities.Keys.ToImmutableArray();

        /// <summary>
        /// This is for editor purposes, we group all entities in "folders" when visualizing them.
        /// This has no effect in the actual game.
        /// </summary>
        public ImmutableDictionary<string, ImmutableArray<Guid>> FetchFolders() => _folders.ToImmutableDictionary();

        /// <summary>
        /// This is for editor purposes, return all the available folder names as an<see cref="IEnumerable{String}"/>
        /// </summary>
        public IEnumerable<string> FetchFolderNames() => _folders.Keys;

        /// <summary>
        /// This is for editor purposes, we group all entities in "folders" when visualizing them.
        /// This has no effect in the actual game.
        /// </summary>
        public ImmutableDictionary<string, ImmutableArray<Guid>> FetchFilters() => _filterFolders.ToImmutableDictionary();

        /// <summary>
        /// Track each group that an entity belongs. Used for speeding up removing entities
        /// and moving them around.
        /// </summary>
        [Serialize]
        private readonly Dictionary<Guid, string> _entitiesToFolder = new();

        /// <summary>
        /// Track each group that an entity belongs. Used for speeding up removing entities
        /// and moving them around.
        /// </summary>
        [Serialize]
        private readonly Dictionary<Guid, string> _entitiesToFilter = new();

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

        /// <summary>
        /// Create a new instance of the world based on this world asset.
        /// </summary>
        /// <param name="camera">Camera which will be used for this world.</param>
        /// <param name="startingSystems">List of default starting assets, if any. This will be used for diagnostics systems, for example.</param>
        public MonoWorld CreateInstance(Camera2D camera, ImmutableArray<(Type, bool)> startingSystems) => CreateInstance(camera, FetchInstances(), startingSystems);

        /// <summary>
        /// Create a new instance of the world based on this world asset.
        /// </summary>
        /// <param name="savedInstance">Saved world instance to start from.</param>
        /// <param name="camera">Camera which will be used for this world.</param>
        /// <param name="startingSystems">List of default starting assets, if any. This will be used for diagnostics systems, for example.</param>
        public MonoWorld CreateInstanceFromSave(SavedWorld savedInstance, Camera2D camera, ImmutableArray<(Type, bool)> startingSystems) => CreateInstance(camera, savedInstance.FetchInstances(), startingSystems);

        private MonoWorld CreateInstance(Camera2D camera, ImmutableArray<EntityInstance> instances, ImmutableArray<(Type, bool)> startingSystems)
        {
            List<(ISystem, bool)> systemInstances = new();

            // Actually instantiate and add each of our system types.
            ImmutableArray<(Type system, bool isActive)> allSystemTypes = FetchAllSystems().AddRange(startingSystems);
            foreach (var (type, isActive) in allSystemTypes)
            {
                if (type is null)
                {
                    // Likely a debug system, skip!
                    continue;
                }

                if (Activator.CreateInstance(type) is ISystem system)
                {
                    systemInstances.Add((system, isActive));
                }
                else
                {
                    GameLogger.Error($"The {type} is not a valid system!");
                }
            }

            MonoWorld world = new(systemInstances, camera, worldAssetGuid: Guid);
            CreateAllEntities(world, instances);

            return world;
        }

        public ImmutableArray<(Type system, bool isActive)> FetchAllSystems()
        {
            var systems = ImmutableArray.CreateBuilder<(Type System, bool Enabled)>();

            // First, let's add our own systems - easy!
            systems.AddRange(_systems);

            // Now, let's fetch each of our features...
            foreach (var feature in _features)
            {
                FeatureAsset featureAsset = Game.Data.GetAsset<FeatureAsset>(feature.guid);
                systems.AddRange(featureAsset.FetchAllSystems(feature.isActive));
            }

            if (_systemsToRemove.Length != 0)
            {
                // Remove all the systems that do not apply.
                HashSet<Type> checkSystemsToBeRemoved = [.. _systemsToRemove];
                systems.RemoveAll(s => checkSystemsToBeRemoved.Contains(s.System));
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
            // As of today, this only tracks *parent* entities.
            Dictionary<Guid, int> instancesToEntities = new();

            foreach (EntityInstance e in instances)
            {
                int id = IWorldAsset.TryCreateEntityInWorld(world, e);
                instancesToEntities.Add(e.Guid, id);
            }

            PostProcessEntities(world, instancesToEntities);
        }

        /// <summary>
        /// This makes any fancy post process once all entities were created in the world.
        /// This may trigger reactive components within the world.
        /// </summary>
        /// <param name="world">World that entities were created.</param>
        /// <param name="instancesToEntities">A map of each serialized guid to an entity id in the world.</param>
        protected static void PostProcessEntities(World world, Dictionary<Guid, int> instancesToEntities)
        {
            if (world.TryGetUniqueEntityInstanceToEntityLookup() is not null)
            {
                // Most likely, we are reloading a saved world. Do not post process this.
                return;
            }

            ImmutableArray<Entity> entities = world.GetActivatedAndDeactivatedEntitiesWith(typeof(GuidToIdTargetComponent));
            foreach (Entity e in entities)
            {
                Guid guid = e.GetGuidToIdTarget().Target;
                e.RemoveGuidToIdTarget();

                if (!instancesToEntities.TryGetValue(guid, out int id))
                {
                    GameLogger.Error($"Tried to reference an entity with guid '{guid}' that is not available in world. " +
                        "Are you trying to access a child entity, which is not supported yet?");

                    continue;
                }

                e.SetIdTarget(id);
            }

            // Now, iterate over all the collection entities.
            entities = world.GetActivatedAndDeactivatedEntitiesWith(typeof(GuidToIdTargetCollectionComponent));
            foreach (Entity e in entities)
            {
                ImmutableArray<GuidId> guids = e.GetGuidToIdTargetCollection().Collection;
                e.RemoveGuidToIdTargetCollection();

                var builder = ImmutableDictionary.CreateBuilder<string, int>();
                foreach (GuidId guidId in guids)
                {
                    if (!instancesToEntities.TryGetValue(guidId.Target, out int id))
                    {
                        GameLogger.Error($"Tried to reference an entity with guid '{guidId.Target}' that is not available in world. " +
                            "Are you trying to access a child entity, which is not supported yet?");

                        continue;
                    }

                    builder[guidId.Id] = id;
                }

                e.SetIdTargetCollection(builder.ToImmutable());
            }

            // Preprocess the quest tracker
            ImmutableArray<Entity> quests = world.GetActivatedAndDeactivatedEntitiesWith(typeof(QuestTrackerComponent));
            foreach (Entity e in quests)
            {
                var questsStages = ImmutableArray.CreateBuilder<QuestStageRuntime>();

                foreach (var quest in e.GetQuestTracker().QuestStages)
                {
                    var actions = ImmutableArray.CreateBuilder<MurderTargetedRuntimeAction>();
                    foreach (var action in quest.Actions)
                    {
                        if (!instancesToEntities.TryGetValue(action.Target, out int actionId))
                        {
                            GameLogger.Error($"Tried to reference an entity with guid '{action.Target}' that is not available in world. " +
                                "Are you trying to access a child entity, which is not supported yet?");

                            continue;
                        }
                        actions.Add(action.Bake(actionId));
                    }
                    questsStages.Add(new QuestStageRuntime(quest.Requirements, actions.ToImmutable()));
                }

                e.SetQuestTrackerRuntime(questsStages.ToImmutable());
                e.RemoveQuestTracker();
            }

            // Preprocess cutscenes so we can use efficient dictionaries instead of arrays.
            ImmutableArray<Entity> cutscenes = world.GetActivatedAndDeactivatedEntitiesWith(typeof(CutsceneAnchorsEditorComponent));
            foreach (Entity e in cutscenes)
            {
                ImmutableArray<AnchorId> anchors = e.GetCutsceneAnchorsEditor().Anchors;
                e.RemoveCutsceneAnchorsEditor();

                var builder = ImmutableDictionary.CreateBuilder<string, Anchor>();
                foreach (AnchorId anchor in anchors)
                {
                    builder[anchor.Id] = anchor.Anchor;
                }

                e.SetCutsceneAnchors(builder.ToImmutable());
            }

            // Load all events from the asset.
            ImmutableDictionary<Guid, GameAsset> assets = Game.Data.FilterAllAssets(typeof(WorldEventsAsset));
            foreach ((_, GameAsset asset) in assets)
            {
                if (asset is not WorldEventsAsset worldEvents)
                {
                    GameLogger.Error("How this is not a world event asset?");

                    // How this happened?
                    continue;
                }

                foreach (TriggerEventOn trigger in worldEvents.Watchers)
                {
                    if (trigger.World is Guid guid && guid != world.Guid())
                    {
                        // Not meant to this world.
                        continue;
                    }

                    world.AddEntity(trigger.CreateComponents());
                }
            }

            // Keep track of the instances <-> entity map in order to do further processing.
            world.AddEntity(new InstanceToEntityLookupComponent(instancesToEntities));
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
            _entities[e.Guid] = e;
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

            _entitiesToFolder.Remove(instanceGuid);
        }

        public void UpdateSystems(ImmutableArray<(Type systemType, bool isActive)> systems) => _systems = systems;
        public void UpdateFeatures(ImmutableArray<(Guid guid, bool isActive)> features) => _features = features;
        public void UpdateSystemsToRemove(ImmutableArray<Type> systems) => _systemsToRemove = systems;

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

        public bool MoveToGroup(string? targetGroup, Guid instance, int targetPosition)
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
                    // Target folder was not found, so create one!
                    AddGroup(targetGroup);

                    instances = ImmutableArray<Guid>.Empty;
                }

                if (targetPosition == -1)
                {
                    _folders[targetGroup] = instances.Add(instance);
                }
                else
                {
                    _folders[targetGroup] = instances.Insert(targetPosition, instance);
                }

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

        public ImmutableArray<Guid> FetchEntitiesOfSoundGroup(string name) => _filterFolders[name];
        public ImmutableArray<Guid> FetchEntitiesOfGroup(string name)
        {
            if (_folders.ContainsKey(name))
            {
                return _folders[name];
            }

            return [];
        }

        public int GroupsCount() => _folders.Count;

        /// <summary>
        /// Add a new filter to group entities.
        /// </summary>
        public bool AddFilter(string name)
        {
            if (_filterFolders.ContainsKey(name))
            {
                return false;
            }

            _filterFolders[name] = ImmutableArray<Guid>.Empty;
            return true;
        }

        /// <summary>
        /// Delete a new filter to group entities.
        /// </summary>
        public bool DeleteFilter(string name)
        {
            if (!_filterFolders.ContainsKey(name))
            {
                return false;
            }

            _filterFolders.Remove(name);
            return true;
        }

        public void MoveToFilter(string? targetFilter, Guid instance, int targetPosition)
        {
            ImmutableArray<Guid> instances;

            // First, remove from any prior group, if it belong to one.
            if (_entitiesToFilter.TryGetValue(instance, out string? fromGroup) &&
                _filterFolders.TryGetValue(fromGroup, out instances))
            {
                _filterFolders[fromGroup] = instances.Remove(instance);
            }

            if (targetFilter is null)
            {
                _entitiesToFilter.Remove(instance);
                return;
            }

            // Now, move to the target group.
            if (!_filterFolders.TryGetValue(targetFilter, out instances))
            {
                instances = [];

                _filterFolders[targetFilter] = instances;
            }

            if (targetPosition == -1)
            {
                _filterFolders[targetFilter] = instances.Add(instance);
            }
            else
            {
                _filterFolders[targetFilter] = instances.Insert(targetPosition, instance);
            }

            _entitiesToFilter[instance] = targetFilter;
        }

        public ImmutableArray<Guid> FetchEntitiesGuidInFilter(string targetFilter)
        {
            if (_filterFolders.TryGetValue(targetFilter, out ImmutableArray<Guid> entitiesGuid))
            {
                return entitiesGuid;
            }

            return ImmutableArray<Guid>.Empty;
        }

        public List<IEntity> FetchEntitiesInFilter(string targetFilter)
        {
            List<IEntity> result = new();
            if (_filterFolders.TryGetValue(targetFilter, out ImmutableArray<Guid> entitiesGuid))
            {
                foreach (Guid g in entitiesGuid)
                {
                    if (_entities.TryGetValue(g, out EntityInstance? instance))
                    {
                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        public bool IsOnFilter(Guid g) => _entitiesToFilter.ContainsKey(g);
    }
}