using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// This is a component with a collection of entities tracked in the world.
    /// </summary>
    [RuntimeOnly]
    [PersistOnSave]
    [KeepOnReplace]
    public readonly struct IdTargetCollectionComponent : IComponent
    {
        /// <summary>
        /// Id of the target entity.
        /// </summary>
        public readonly ImmutableDictionary<string, int> Targets =
            ImmutableDictionary<string, int>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase);

        public IdTargetCollectionComponent(ImmutableDictionary<string, int> targets) => Targets = targets;


        public Entity? TryGetEntiy(World world, string id)
        {
            if (Targets.TryGetValue(id, out var entityId))
            {
                return world.TryGetEntity(entityId);
            }

            return null;
        }
    }
}