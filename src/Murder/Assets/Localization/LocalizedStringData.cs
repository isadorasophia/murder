namespace Murder.Assets.Localization;

public readonly struct LocalizedStringData
{
    public readonly string String { get; init; } = string.Empty;

    /// <summary>
    /// Total of references to this string data.
    /// </summary>
    public readonly int Counter { get; init; } = 1;

    public LocalizedStringData() { }
}
