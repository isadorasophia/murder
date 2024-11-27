using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Editor.ImGuiExtended;
using Murder.Services;
using static Murder.Editor.ImGuiExtended.SearchBox;

namespace Murder.Editor.Services;

internal static class EditorLocalizationServices
{
    public static LocalizedString? SearchLocalizedString()
    {
        LocalizationAsset localization = LocalizationServices.GetCurrentLocalization();

        SearchBoxSettings<Guid> settings = new(initialText: "Create localized string") 
        { 
            DefaultInitialization  = ("New localized string", Guid.Empty) 
        };

        Lazy<Dictionary<string, Guid>> candidates = new(() => 
        {
            Dictionary<string, Guid> result = [];

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

        if (SearchBox.Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out Guid result))
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

    public static LocalizedString? AddNewResource()
    {
        LocalizedString result = new(Guid.NewGuid());

        LocalizationAsset asset = Game.Data.Localization;
        asset.AddResource(result.Id);

        EditorServices.SaveAssetWhenSelectedAssetIsSaved(asset.Guid);
        asset.FileChanged = true;

        return result;
    }

    private static void AddExistingResource(Guid g)
    {
        LocalizationAsset asset = Game.Data.Localization;
        asset.AddResource(g);

        EditorServices.SaveAssetWhenSelectedAssetIsSaved(asset.Guid);
        asset.FileChanged = true;
    }
}
