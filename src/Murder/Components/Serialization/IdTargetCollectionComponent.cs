using Bang.Components;
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
    public readonly struct IdTargetCollectionComponent : IComponent
    {
        /// <summary>
        /// Id of the target entity.
        /// </summary>
        public readonly ImmutableDictionary<string, int> Targets = 
            ImmutableDictionary<string, int>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase);

        public IdTargetCollectionComponent(ImmutableDictionary<string, int> targets) => Targets = targets;
    }
}
