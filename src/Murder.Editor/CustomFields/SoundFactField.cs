using ImGuiNET;
using Murder.Core.Sounds;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(SoundFact))]
    internal class SoundFactField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            SoundFact? fact = (SoundFact?)fieldValue;

            ImGui.BeginChild("selection_sounds_fact", new System.Numerics.Vector2(x: 300, y: ImGui.GetFontSize() * 1.5f));

            // -- Facts across all blackboards --
            if (SearchBox.SearchSoundFacts("sounds_fact_search", fact) is SoundFact newFact)
            {
                fact = newFact;
                modified = true;
            }

            ImGui.EndChild();

            return (modified, fact);
        }
    }
}