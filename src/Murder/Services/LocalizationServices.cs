using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Diagnostics;

namespace Murder.Services
{
    [Flags]
    public enum LocalizationFlags
    {
        None,
        ForceDefault
    }

    public static class LocalizationServices
    {
        public static LocalizationAsset GetCurrentLocalization() => Game.Data.Localization;

        public static string? TryGetLocalizedString(LocalizedString? localized) =>
            localized is null ? null : GetLocalizedString(localized.Value);

        public static string GetLocalizedString(LocalizedString localized, LocalizationFlags flags = LocalizationFlags.None)
        {
            if (localized.OverrideText is not null)
            {
                return localized.OverrideText;
            }

            if (localized.Id == Guid.Empty)
            {
                return string.Empty;
            }

            LocalizationAsset asset = flags.HasFlag(LocalizationFlags.ForceDefault) ?
                Game.Data.GetDefaultLocalization() :
                Game.Data.Localization;

            LocalizedStringData? data = asset.TryGetResource(localized.Id) ??
                Game.Data.GetDefaultLocalization().TryGetResource(localized.Id);

            if (data is null)
            {
                GameLogger.Error($"Unable to acquire resource for {localized.Id}.");
                return string.Empty;
            }

            return data.Value.String;
        }

        public static bool IsTextWrapOnlyOnSpace()
        {
            if (Game.Data.CurrentLocalization.Id == LanguageId.Japanese || 
                Game.Data.CurrentLocalization.Id == LanguageId.Chinese)
            {
                return false;
            }

            return true;
        }
    }
}
