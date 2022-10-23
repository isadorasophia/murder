using InstallWizard.Dungeons;
using Editor.Gui;
using Editor.Reflection;
using Editor.Util;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<DungeonObjectKind, ImmutableArray<DungeonAssetPlacer>>), priority: 10)]
    internal class DictionaryDungeonAssetField : DictionaryField<DungeonObjectKind, ImmutableArray<DungeonAssetPlacer>>
    {
        protected override bool Add(IList<DungeonObjectKind> candidates, [NotNullWhen(true)] out (DungeonObjectKind Key, ImmutableArray<DungeonAssetPlacer> Value)? element)
        {
            if (SearchBox.SearchEnum(candidates, out DungeonObjectKind kind))
            {
                element = (kind, ImmutableArray<DungeonAssetPlacer>.Empty);
                return true;
            }

            element = (default, default);
            return false;
        }

        protected override List<DungeonObjectKind> GetCandidateKeys(EditorMember _, IDictionary<DungeonObjectKind, ImmutableArray<DungeonAssetPlacer>> fieldValue)
        {
            Array allValues = Enum.GetValues(typeof(DungeonObjectKind));

            List<DungeonObjectKind> candidates = new();
            foreach (DungeonObjectKind kind in allValues)
            {
                if (!fieldValue.ContainsKey(kind))
                {
                    candidates.Add(kind);
                }
            }

            return candidates;
        }

        /// <summary>
        /// This is optionally implemented by dictionaries that have a collection with a type
        /// that depends on its key.
        /// </summary>
        public override Type? CustomElementTypeOfValue(DungeonObjectKind key) => ReflectionHelper.GetDungeonPlacerFor(key);
    }
}
