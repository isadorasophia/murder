using Murder.Attributes;

namespace Murder.Assets;

public readonly struct LocalizedString
{
    /// <summary>
    /// Only set when there is a custom localized asset for this string.
    /// </summary>
    [HideInEditor]
    public readonly Guid? Asset = null;

    public readonly Guid Id = Guid.Empty;

    public LocalizedString() { }

    public LocalizedString(Guid id) => Id = id;
}
