using Murder.Services;

namespace Murder.Assets;

public readonly struct LocalizedString
{
    public readonly Guid Id = Guid.Empty;

    /// <summary>
    /// Used when, for whatever reason, we need to override the data for a localized string
    /// with actual text (usually built in runtime).
    /// </summary>
    public readonly string? OverrideText = null;

    public LocalizedString() { }

    public LocalizedString(Guid id) => Id = id;

    public LocalizedString(string overrideText) => OverrideText = overrideText;

    public static implicit operator string(LocalizedString localizedString) =>
        LocalizationServices.GetLocalizedString(localizedString);

    public override string ToString() =>
        LocalizationServices.GetLocalizedString(this);
}
