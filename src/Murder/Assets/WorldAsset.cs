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

        [JsonProperty]
        private readonly Dictionary<Guid, EntityInstance> _entities = new();

        public ImmutableArray<(Type systemType, bool isActive)> Systems => _systems;
        public ImmutableArray<(Guid guid, bool isActive)> Features => _features;

        public ImmutableArray<Guid> Instances => _entities.Keys.ToImmutableArray();

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
                }
            }
        }

        public void AddInstance(EntityInstance e)
        {
            _entities.Add(e.Guid, e);
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
        }

        public void UpdateSystems(ImmutableArray<(Type systemType, bool isActive)> systems) => _systems = systems;
        public void UpdateFeatures(ImmutableArray<(Guid guid, bool isActive)> features) => _features = features;
    }
}
