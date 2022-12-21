using Bang;
using Murder.Diagnostics;
using Murder.Prefabs;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public interface IWorldAsset
    {
        public Guid WorldGuid { get; }
        
        public EntityInstance? TryGetInstance(Guid instanceGuid);

        public ImmutableArray<Guid> Instances { get; }

        public int TryCreateEntityInWorld(World world, Guid instance)
        {
            int id = -1;

            if (TryGetInstance(instance) is EntityInstance e)
            {
                id = TryCreateEntityInWorld(world, e);
            }
            else
            {
                GameLogger.Error($"Unable to find {instance} in this world asset.");
            }

            return id;
        }

        internal static int TryCreateEntityInWorld(World world, EntityInstance e)
        {
            try
            {
                return e.Create(world);
            }
            catch
            {
                GameLogger.Error($"Cannot find asset for entity {e.Name}.");
            }

            return -1;
        }
    }
}
