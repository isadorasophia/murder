namespace Murder.Core.Dialogs;

/// <summary>
/// This represents an item in the dialog that has been manually modified by the user
/// and should be persisted.
/// </summary>
public readonly struct DialogItemId
{
    public readonly int SituationId = 0;
    public readonly int DialogId = 0;
    public readonly int ItemId = 0;

    public DialogItemId(int situation, int dialog, int id)
    {
        SituationId = situation;
        DialogId = dialog;
        ItemId = id;
    }
}

/// <summary>
/// This represents an item id in the dialog that has been manually modified by the user
/// and should be persisted.
/// </summary>
public readonly struct DialogueId
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