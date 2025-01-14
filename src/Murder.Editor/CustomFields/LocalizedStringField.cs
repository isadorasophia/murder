using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Services;
using Murder.Prefabs;
using Murder.Services;

namespace Murder.Editor.CustomFields;

[CustomFieldOf(typeof(LocalizedString))]
internal class LocalizedStringField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        bool modified = false;
        LocalizedString? localizedString = (LocalizedString?)fieldValue;

        LocalizationAsset localization = Game.Data.GetDefaultLocalization();

        string searchId = $"Search_localized#{member.Name}";

        if (localizedString is null || localizedString.Value.Id == Guid.Empty)
        {
            bool create = false;

            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
            if (ImGui.Button("\uf002"))
            {
                ImGui.OpenPopup(searchId);
            }

            ImGuiHelpers.HelpTooltip("Search existing localized string");

            ImGui.SameLine();
            if (ImGui.Button("\uf15e"))
            {
                create = true;
            }

            ImGuiHelpers.HelpTooltip("Create localized string");
            ImGui.PopStyleColor();

            if (create)
            {
                return (true, EditorLocalizationServices.AddNewResource());
            }
        }
        else if (localizedString.Value.Id is Guid guid)
        {
            if (ImGuiHelpers.DeleteButton($"localized_{member.Name}"))
            {
                localization.RemoveResource(guid);

                localizedString = default;
                modified = true;
            }

            ImGui.SameLine();

            if (ImGuiHelpers.IconButton('\uf2ea', $"localized_{member.Name}"))
            {
                localizedString = default;
                modified = true;
            }

            ImGuiHelpers.HelpTooltip("Discard localized string without deleting");
            ImGui.SameLine();

            // == Notes ==
            if (ImGuiHelpers.BlueIcon('\uf10d', $"note_b_{guid}"))
            {
                ImGui.OpenPopup($"notes_{guid}");
            }

            EditorLocalizationServices.DrawNotesPopup(guid);
            ImGui.SameLine();
        }

        if (ImGui.BeginPopup(searchId))
        {
            ImGui.Dummy(new System.Numerics.Vector2(300, 0));

            if (EditorLocalizationServices.SearchLocalizedString() is LocalizedString localizedResult)
            {
                localizedString = localizedResult;

                ImGui.CloseCurrentPopup();
            }

            ImGui.Dummy(new System.Numerics.Vector2(300, 0));
            ImGui.EndPopup();

            return (true, localizedString);
        }

        if (localizedString is null || localizedString.Value.Id == Guid.Empty)
        {
            return (modified, localizedString);
        }

        if (localization.TryGetResource(localizedString.Value.Id) is not LocalizedStringData data)
        {
            return (true, default);
        }

        string text = data.String;
        if (AttributeExtensions.IsDefined(member, typeof(MultilineAttribute)))
        {
            modified = ImGui.InputTextMultiline($"##{data.Guid}", ref text, 1024, new(-1, 75));
        }
        else
        {
            modified = ImGui.InputText($"##{data.Guid}", ref text, 2048);
        }

        if (modified)
        {
            data = data with { String = text };

            localization.SetResource(data);
            modified = true;

            EditorServices.SaveAssetWhenSelectedAssetIsSaved(localization.Guid);
            localization.FileChanged = true;
        }

        return (modified, localizedString);
    }
}
