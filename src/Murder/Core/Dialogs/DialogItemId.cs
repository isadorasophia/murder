using Bang.Components;

namespace Murder.Core.Dialogs;

/// <summary>
/// This represents an item id in the dialog that has been manually modified by the user
/// and should be persisted.
/// </summary>
public readonly record struct DialogueId
{
    public readonly string UniqueName = string.Empty;
    public readonly int DialogId = 0;
    public readonly int ItemId = 0;

    public DialogueId(string name, int dialog, int id)
    {
        UniqueName = name;
        DialogId = dialog;
        ItemId = id;
    }
}

[Flags]
public enum LineInfoProperties
{
    None = 0,
    SkipDefaultPortraitSound = 1
}

public readonly struct LineInfo
{
    public readonly Guid Speaker { get; init; } = Guid.Empty;
    public readonly string? Portrait { get; init; } = null;

    public string? Event { get; init; } = null;

    /// <summary>
    /// Component modified within a dialog.
    /// </summary>
    public IComponent? Component { get; init; } = null;

    public LineInfoProperties Flags { get; init; } = LineInfoProperties.None;

    public LineInfo() { }

    public bool Empty => Speaker == Guid.Empty && Portrait is null && Event is null && Component is null;
}

public readonly struct DialogueLineInfo
{
    public readonly DialogueId Id = new();
    public readonly LineInfo Info { get; init; } = new();

    public DialogueLineInfo() { }

    public DialogueLineInfo(DialogueId id) => Id = id;
}