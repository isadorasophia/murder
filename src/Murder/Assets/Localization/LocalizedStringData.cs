namespace Murder.Assets.Localization;

public readonly struct LocalizedStringData
{
    public readonly Guid Guid = Guid.Empty;

    public readonly string String { get; init; } = string.Empty;

    /// <summary>
    /// Total of references to this string data.
    /// </summary>
    public readonly int? Counter { get; init; } = null;

    /// <summary>
    /// Any notes relevant to this string.
    /// </summary>
    public readonly string? Notes { get; init; } = null;

    public readonly bool IsGenerated { get; init; } = false;

    public LocalizedStringData() { }

    public LocalizedStringData(Guid guid) => Guid = guid;
}
