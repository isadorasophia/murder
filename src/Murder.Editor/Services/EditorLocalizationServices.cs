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

    public static void AddExistingResource(Guid g)
    {
        LocalizationAsset asset = Game.Data.Localization;
        asset.AddResource(g);

        Architect.EditorData.SaveAsset(asset);
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
            Dictionary<string, Guid> result = new()
            {
                ["New localized string"] = Guid.Empty
            };

            foreach (LocalizedStringData data in localization.Resources)
            {
                if (data.IsGenerated)
                {
                    // Don't trust generated strings!
                    continue;
                }

                string key;
                if (data.String.Length > 32)
                {
                    key = data.String[..20] + "...";
                }
                else
                {
                    key = data.String;
                }

                if (result.ContainsKey(key))
                {
                    continue;
                }

                result[key] = data.Guid;
            }

            return result;
        });

        if (SearchBox.Search(id: "s_", hasInitialValue: false, selected, values: candidates, out Guid result))
        {
            if (result == Guid.Empty)
            {
                return AddNewResource();
            }

            AddExistingResource(result);
            return new(result);
        }

        return null;
    }
}
