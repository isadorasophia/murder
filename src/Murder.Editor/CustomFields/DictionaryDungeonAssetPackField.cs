using InstallWizard.Dungeons;
using Editor.Gui;
using Editor.Reflection;
using Editor.Util;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<DungeonObjectKind, ImmutableArray<Guid>>), priority: 10)]
    internal class DictionaryDungeonAssetPackField : DictionaryField<DungeonObjectKind, ImmutableArray<Guid>>
    {
        protected override bool Add(IList<DungeonObjectKind> candidates, [NotNullWhen(true)] out (DungeonObjectKind Key, ImmutableArray<Guid> Value)? element)
        {
            if (SearchBox.SearchEnum(candidates, out DungeonObjectKind kind))
            {
                element = (kind, ImmutableArray<Guid>.Empty);
                return true;
            }

            element = (default, default);
            return false;
        }

        protected override List<DungeonObjectKind> GetCandidateKeys(EditorMember _, IDictionary<DungeonObjectKind, ImmutableArray<Guid>> fieldValue)
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
    }
}
