using Bang.Entities;
using Murder.Components;
using System.Collections.Immutable;

namespace Bang
{
    /// <summary>
    /// Additional lookup class on top of the Bang generated one. Needed for adding
    /// <see cref="IMurderTransformComponent"/> to the relative component lookup table with the correct id.
    /// </summary>
    public class MurderTransformComponentsLookup : MurderComponentsLookup
    {
        public const int MurderTransformNextLookupId = MurderNextLookupId;

        public MurderTransformComponentsLookup()
        {
            ComponentsIndex = base.ComponentsIndex.Concat(_relativeComponents).ToImmutableDictionary();
        }

        private static readonly ImmutableDictionary<Type, int> _relativeComponents = new Dictionary<Type, int>
        {
            { typeof(IMurderTransformComponent), BangComponentTypes.Transform }
        }.ToImmutableDictionary();
    }
}