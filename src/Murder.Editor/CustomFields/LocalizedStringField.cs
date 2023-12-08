using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Services;
using Murder.Services;

namespace Murder.Editor.CustomFields;

[CustomFieldOf(typeof(LocalizedString))]
internal class LocalizedStringField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        bool modified = false;
        LocalizedString? localizedString = (LocalizedString?)fieldValue;

        LocalizationAsset localization = LocalizationServices.GetCurrentLocalization();

        if (localizedString is null || localizedString.Value.Id == Guid.Empty)
        {
            if (EditorLocalizationServices.SearchLocalizedString() is LocalizedString localizedResult)
            {
                return (true, localizedResult);
            }
        }
        else
        {
            if (ImGuiHelpers.IconButton('\uf2ea', $"localized_{member.Name}"))
            {
                localization.RemoveResource(localizedString.Value.Id);

                localizedString = default;
                modified = true;
            }

            ImGuiHelpers.HelpTooltip("Remove localized string");
            ImGui.SameLine();
        }

        if (localizedString is null || localizedString.Value.Id == Guid.Empty)
        {
            return (modified, localizedString);
        }

        if (localization.TryGetResource(localizedString.Value.Id) is not LocalizedStringData data)
        {
            return (true, default);
        }

        if (DrawValue(ref data, nameof(LocalizedStringData.String)))
        {
            localization.SetResource(data);
            modified = true;

            Architect.EditorData.SaveAsset(localization);
        }

        return (modified, localizedString);
    }
}
