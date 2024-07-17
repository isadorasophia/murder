namespace Murder.Editor.Core;

public readonly struct StartPlayGameInfo
{
    public readonly bool IsQuickplay { get; init; } = false;

    public readonly Guid? StartingScene { get; init; } = null;

    public readonly int? SaveSlot { get; init; } = null;

    public readonly Guid? LoadStateFrom { get; init; } = null;

    public StartPlayGameInfo() { }
}
