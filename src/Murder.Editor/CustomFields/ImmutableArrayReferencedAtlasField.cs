using ImGuiNET;
using Murder.Assets;
using Murder.Data;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using static System.Net.Mime.MediaTypeNames;

namespace Murder.Editor.CustomFields;

[CustomFieldOf(typeof(ImmutableArray<ReferencedAtlas>))]
internal class ImmutableArrayReferencedAtlasField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        ImmutableArray<ReferencedAtlas> current = (ImmutableArray<ReferencedAtlas>)fieldValue!;
        return ProcessAtlasName(current);
    }

    private (bool modified, object? result) ProcessAtlasName(ImmutableArray<ReferencedAtlas> current)
    {
        bool modified = false;

        var builder = ImmutableArray.CreateBuilder<ReferencedAtlas>();
        builder.AddRange(current);

        HashSet<int> missingNames = new();

        HashSet<string>? names = [.. Game.Data.LoadedAtlasses.Keys];
        if (names is not null)
        {
            for (int i = 0; i < current.Count(); i++)
            {
                string text = current[i].Id;

                bool removed = names.Remove(text);
                if (!removed)
                {
                    missingNames.Add(i);
                }
            }
        }

        for (int i = 0; i < current.Count(); i++)
        {
            string text = current[i].Id;

            bool @checked = current[i].UnloadOnExit;
            if (ImGui.Checkbox($"##unload_{i}", ref @checked))
            {
                modified = true;
                builder[i] = new(builder[i].Id, @checked);
            }

            ImGuiHelpers.HelpTooltip("Unload on exit");
            ImGui.SameLine();

            if (ImGuiHelpers.DeleteButton($"Delete target name##{i}"))
            {
                modified = true;
                builder.RemoveAt(i);
            }

            ImGui.SameLine();

            if (names is null)
            {
                ImGui.Text(text);
            }
            else
            {
                bool isMissing = missingNames.Contains(i);
                if (isMissing)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Red);
                }

                if (StringField.ProcessStringCombo($"replace_target_{i}", ref text, names))
                {
                    modified = true;
                    builder[i] = new(text, current[i].UnloadOnExit);
                }

                if (isMissing)
                {
                    ImGui.PopStyleColor();
                }
            }

            // Remove element that has been added.
            names?.Remove(text);
        }

        if (names is not null && names.Any())
        {
            string element = "Select an atlas";
            if (StringField.ProcessStringCombo("array_targetname_new", ref element, names))
            {
                modified = true;
                builder.Add(new(element, unloadOnExit: true));
            }
        }
        else if (current.Length == 0)
        {
            ImGui.TextColored(Game.Profile.Theme.Faded, "No atlas :-(");
        }

        return (modified, result: builder.ToImmutable());
    }
}
