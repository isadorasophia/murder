using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Diagnostics;

namespace Murder.Editor.Services;

internal static class EditorLocalizationServices
{
    public static LocalizedString? AddNewResource()
    {
        if (Game.Data.TryGetAsset<LocalizationAsset>(Game.Profile.DefaultLocalization) is not LocalizationAsset asset)
        {
            GameLogger.Error("Default localization asset is not set in game profile.");
            return null;
        }

        LocalizedString result = new(Guid.NewGuid());
        asset.AddResource(result.Id);

        Architect.EditorData.SaveAsset(asset);
        return result;
    }
}
