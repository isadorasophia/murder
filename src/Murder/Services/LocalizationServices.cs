using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class LocalizationServices
    {
        public static LocalizationAsset GetCurrentLocalization() => Game.Data.Localization;

        public static string GetLocalizedString(LocalizedString localized)
        {
            LocalizationAsset asset = Game.Data.Localization;
            if (asset.TryGetResource(localized.Id) is not LocalizedStringData data)
            {
                GameLogger.Error($"Unable to acquire resource for {localized.Id}.");
                return string.Empty;
            }

            return data.String;
        }
    }
}
