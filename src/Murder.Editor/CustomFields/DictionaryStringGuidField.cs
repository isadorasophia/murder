using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<string, Guid>))]
    internal class DictionaryStringGuidField : DictionaryField<string, Guid>
    {
        protected override bool AddNewKey(EditorMember member, ref IDictionary<string, Guid> dictionary)
        {
            if (ImGuiHelpers.IconButton('\uf055', $"##add_key_{member.Name}"))
            {
                if (dictionary is ImmutableDictionary<string, Guid> immutable)
                {
                    dictionary = immutable.Add(string.Empty, Guid.Empty);
                    return true;
                }
            }

            return false;
        }

        protected override bool CanModifyKeys() => true;

        protected override bool Add(IList<string> candidates, [NotNullWhen(true)] out (string Key, Guid Value)? element)
        {
            element = (string.Empty, Guid.Empty);
            return true;
        }

        protected override List<string> GetCandidateKeys(EditorMember member, IDictionary<string, Guid> fieldValue)
        {
            return new List<string> { string.Empty };
        }
    }
}
