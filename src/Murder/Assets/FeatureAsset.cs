using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets
{
    public class FeatureAsset : GameAsset
    {
        public override char Icon => '';
        public override string EditorFolder => "#Features";
        public override Vector4 EditorColor => "#34ebcf".ToVector4Color();

        public ImmutableArray<(Type systemType, bool isActive)> SystemsOnly => _systems;

        public ImmutableArray<(Guid feature, bool isActive)> FeaturesOnly => _features;

        /// <summary>
        /// Whether this should always be added when running with diagnostics (e.g. editor).
        /// This will NOT be serialized into the world.
        /// </summary>
        public bool IsDiagnostics = false;

        /// <summary>
        /// Map of all the systems and whether they are active or not.
        /// </summary>
        [Bang.Serialize]
        private ImmutableArray<(Type systemType, bool isActive)> _systems = ImmutableArray<(Type systemType, bool isActive)>.Empty;

        [Bang.Serialize]
        private ImmutableArray<(Guid feature, bool isActive)> _features = ImmutableArray<(Guid feature, bool isActive)>.Empty;

        public bool HasSystems
        {
            get
            {
                if (_systems.Count() > 0)
                    return true;

                foreach (var feature in _features)
                {
                    if (!feature.isActive)
                        continue;

                    if (Game.Data.GetAsset<FeatureAsset>(feature.feature).HasSystems)
                        return true;
                }

                return false;
            }
        }

        public void SetSystems(IList<(Type systemType, bool isActive)> newList)
        {
            _systems = newList.ToImmutableArray();
        }

        public void SetFeatures(IList<(Guid feature, bool isActive)> newSystemsList)
        {
            _features = newSystemsList.ToImmutableArray();
        }

        public ImmutableArray<(Type systemType, bool isActive)> FetchAllSystems(bool enabled)
        {
            var builder = ImmutableArray.CreateBuilder<(Type systemType, bool isActive)>();

            foreach (var system in _systems)
            {
                builder.Add((system.systemType, system.isActive && enabled));
            }

            foreach (var guid in _features)
            {
                FeatureAsset? asset = Game.Data.TryGetAsset<FeatureAsset>(guid.feature);
                if (asset is null)
                {
                    GameLogger.Warning($"Skipping feature asset of {guid.feature} for {Name}.");
                    continue;
                }

                builder.AddRange(asset.FetchAllSystems(guid.isActive && enabled));
            }

            return builder.ToImmutable();
        }

    }
}