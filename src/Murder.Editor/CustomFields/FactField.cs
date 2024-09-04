using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Fact))]
    internal class FactField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            Fact? fact = (Fact?)fieldValue;

            // -- Facts across all blackboards --
            if (SearchBox.SearchFacts("sounds_search", fact) is Fact newFact)
            {
                fact = newFact;
                modified = true;
            }

            return (modified, fact);
        }
    }
}