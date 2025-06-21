using Bang;
using Bang.Entities;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Geometry;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Core.Particles
{
    public class WorldParticleSystemTracker
    {
        private const int DEFAULT_POOL_SZ = 32;
        private ParticleSystemTracker[] _poolTrackers;

        private int _currentLength = 0;

        /// <summary>
        /// List of all the particle systems that currently exist in the world.
        /// Maps:
        /// [ Entity Id -> Index of tracker in the pool ]
        /// </summary>
        private readonly Dictionary<int, int> _particleSystems = new();

        /// <summary>
        /// List of all the particle systems that currently exist in the world.
        /// Maps:
        /// [ Index of tracker in the pool -> EntityId ]
        /// </summary>
        private readonly Dictionary<int, int> _indexToEntityId = new();

        /// <summary>
        /// List of all the <see cref="_particleSystems"/> that are currently active.
        /// </summary>
        private readonly HashSet<int> _activeParticleSystems = new();

        private int _seed = 0;

        public WorldParticleSystemTracker(int seed = 0)
        {
            _seed = seed;
            _poolTrackers = new ParticleSystemTracker[DEFAULT_POOL_SZ];
        }

        private int Add(in ParticleSystemTracker tracker)
        {
            if (_currentLength >= DEFAULT_POOL_SZ)
            {
                // Resize pool
                ParticleSystemTracker[] newArray = new ParticleSystemTracker[_currentLength * 2];
                Array.Copy(_poolTrackers, newArray, _currentLength);

                _poolTrackers = newArray;
            }

            _poolTrackers[_currentLength] = tracker;
            return _currentLength++;
        }

        private void Remove(int id)
        {
            if (id != _currentLength - 1)
            {
                // Pool the particles back.
                _poolTrackers[id] = _poolTrackers[_currentLength - 1];

                int replacedEntityId = _indexToEntityId[_currentLength - 1];
                _particleSystems[replacedEntityId] = id;
                _indexToEntityId[id] = replacedEntityId;
            }

            _poolTrackers[_currentLength - 1] = default;
            _indexToEntityId.Remove(_currentLength - 1);

            _currentLength--;
        }

        public bool Track(Entity particleEntity)
        {
            if (particleEntity.HasDisableParticleSystem())
            {
                return false;
            }

            if (_particleSystems.ContainsKey(particleEntity.EntityId))
            {
                return false;
            }

            Guid particleGuid = particleEntity.GetParticleSystem().Asset;
            if (Game.Data.TryGetAsset<ParticleSystemAsset>(particleGuid) is not ParticleSystemAsset asset)
            {
                // Invalid guid?
                return false;
            }

            if (!particleEntity.HasDisableParticleSystem())
            {
                _activeParticleSystems.Add(particleEntity.EntityId);
            }

            int index = Add(new(asset.Emitter, asset.Particle, _seed));

            _particleSystems[particleEntity.EntityId] = index;
            _indexToEntityId[index] = particleEntity.EntityId;

            return true;
        }

        public void Untrack(Entity particleEntity)
        {
            if (_particleSystems.ContainsKey(particleEntity.EntityId))
            {
                Remove(_particleSystems[particleEntity.EntityId]);
            }

            _activeParticleSystems.Remove(particleEntity.EntityId);
            _particleSystems.Remove(particleEntity.EntityId);
        }

        public bool Synchronize(Entity particleEntity)
        {
            if (!_particleSystems.ContainsKey(particleEntity.EntityId))
            {
                return Track(particleEntity);
            }

            if (!particleEntity.HasDisableParticleSystem())
            {
                _activeParticleSystems.Add(particleEntity.EntityId);
            }

            Guid particleGuid = particleEntity.GetParticleSystem().Asset;
            if (Game.Data.TryGetAsset<ParticleSystemAsset>(particleGuid) is not ParticleSystemAsset asset)
            {
                // Invalid guid?
                return false;
            }

            int index = _particleSystems[particleEntity.EntityId];
            _poolTrackers[index] = new(asset.Emitter, asset.Particle, _seed++);

            return true;
        }

        public void Activate(int id)
        {
            _activeParticleSystems.Add(id);
        }

        public void Deactivate(int id)
        {
            _activeParticleSystems.Remove(id);
        }

        public bool IsTracking(int id)
        {
            return _activeParticleSystems.Contains(id);
        }

        /// <summary>
        /// Set the alpha for a particle system itself according to the <paramref name="entityId"/>.
        /// </summary>
        public bool SetAlpha(int entityId, float alpha)
        {
            if (!_particleSystems.TryGetValue(entityId, out int particleId))
            {
                return false;
            }

            _poolTrackers[particleId].Alpha = alpha;
            return true;
        }

        public void Step(World world, Rectangle cameraArea)
        {
            for (int i = 0; i < _currentLength; ++i)
            {
                int entityId = _indexToEntityId[i];
                Vector2 position = world.GetEntity(entityId).TryGetMurderTransform()?.GetGlobal()?.Vector2 ?? Vector2.Zero;
                _poolTrackers[i].Step(_activeParticleSystems.Contains(entityId), position, cameraArea ,entityId);
            }
        }

        /// <summary>
        /// Fetch all the active particle trackers.
        /// </summary>
        public ReadOnlySpan<ParticleSystemTracker> FetchActiveParticleTrackers() =>
            new(_poolTrackers, 0, _currentLength);

        internal bool HasParticles(Entity e)
        {
            if (!_particleSystems.TryGetValue(e.EntityId, out int pIndex))
            {
                return false;
            }
            var tracker = _poolTrackers[pIndex];

            return tracker.CurrentTime == 0 || tracker.Particles.Length != 0;
        }
    }
}