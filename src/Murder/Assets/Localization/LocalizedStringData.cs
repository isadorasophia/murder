namespace Murder.Assets.Localization;

public readonly struct LocalizedStringData
{
    public readonly string String { get; init; } = string.Empty;

    /// <summary>
    /// Total of references to this string data.
    /// </summary>
    public readonly int Counter { get; init; } = 1;

    /// <summary>
    /// Any notes relevant to this string.
    /// </summary>
    public readonly string? Notes { get; init; } = null;

    public LocalizedStringData() { }
}
