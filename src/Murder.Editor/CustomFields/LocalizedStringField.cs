using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
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

        if (localizedString is null || localizedString?.Id == Guid.Empty)
        {
            if (ImGui.Button("Create string"))
            {
                localizedString = EditorLocalizationServices.AddNewResource();
                modified = localizedString != null;
            }
        }

        if (localizedString is null || localizedString.Value.Id == Guid.Empty)
        {
            return (modified, localizedString);
        }

        LocalizationAsset localization = LocalizationServices.GetDefaultLocalization();

        LocalizedStringData data = localization.Resources[localizedString.Value.Id];
        if (DrawValue(ref data, nameof(LocalizedStringData.String)))
        {
            localization.SetResource(localizedString.Value.Id, data);
            modified = true;

            Architect.EditorData.SaveAsset(localization);
        }

        return (modified, localizedString);
    }
}
