using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Editor.ImGuiExtended;
using Murder.Services;

namespace Murder.Editor.Services;

internal static class EditorLocalizationServices
{
    public static LocalizedString? AddNewResource()
    {
        LocalizedString result = new(Guid.NewGuid());

        LocalizationAsset asset = Game.Data.Localization;
        asset.AddResource(result.Id);

        Architect.EditorData.SaveAsset(asset);
        return result;
    }

    public static void RemoveResource(Guid id)
    {
        LocalizationAsset asset = Game.Data.Localization;
        asset.RemoveResource(id);

        Architect.EditorData.SaveAsset(asset);
    }

    public static LocalizedString? SearchLocalizedString()
    {
        LocalizationAsset localization = LocalizationServices.GetCurrentLocalization();

        string selected = "Create localized string";

        Lazy<Dictionary<string, Guid>> candidates = new(() => 
        {
            Dictionary<string, Guid> result = localization.Resources.ToDictionary(kv => kv.Value.String, kv => kv.Key);
            result["New localized string"] = Guid.Empty;

            return result;
        });

        if (SearchBox.Search(id: "s_", hasInitialValue: false, selected, values: candidates, out Guid result))
        {
            if (result == Guid.Empty)
            {
                return AddNewResource();
            }

            return new(result);
        }

        return null;
    }
}
