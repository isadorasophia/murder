using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class LocalizationServices
    {
        public static LocalizationAsset GetCurrentLocalization() => Game.Data.Localization;

        public static string GetLocalizedString(LocalizedString? localized)
        {
            if (localized is null)
            {
                return string.Empty;
            }

            if (localized.Value.OverrideText is not null)
            {
                return localized.Value.OverrideText;
            }

            LocalizationAsset asset = Game.Data.Localization;

            LocalizedStringData? data = asset.TryGetResource(localized.Value.Id) ??
                Game.Data.GetDefaultLocalization().TryGetResource(localized.Value.Id);

            if (data is null)
            {
                GameLogger.Error($"Unable to acquire resource for {localized.Value.Id}.");
                return string.Empty;
            }

            return data.Value.String;
        }
    }
}
