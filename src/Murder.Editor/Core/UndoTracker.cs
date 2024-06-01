namespace Murder.Editor.Core;

public class UndoTracker
{
    /// <summary>
    /// Minimum time before grouping actions.
    /// </summary>
    private const float TimeBetweenGroupingActions = .5f;

    private readonly Stack<UndoableAction> _undo;
    private readonly Stack<UndoableAction> _redo;

    public UndoTracker(int capacity)
    {
        _undo = new(capacity);
        _redo = new(capacity);
    }

    public void Track(UndoableAction action)
    {
        if (_undo.TryPeek(out UndoableAction? peekAction) && peekAction.AddedAt + TimeBetweenGroupingActions >= action.AddedAt)
        {
            peekAction.Append(action);
            return;
        }

        _undo.Push(action);
        _redo.Clear();
    }

    public void Track(Action @do, Action @undo) => Track(new UndoableAction(@do, @undo, Game.NowUnscaled));

    public void Undo()
    {
        if (_undo.Count == 0)
        {
            return;
        }

        UndoableAction action = _undo.Pop();
        action.Undo();

        _redo.Push(action);
    }

    public void Redo()
    {
        if (_redo.Count == 0)
        {
            return;
        }

        UndoableAction action = _redo.Pop();
        action.Perform();

        _undo.Push(action);
    }
}

public class UndoableAction
{
    private Action _do;
    private Action _undo;

    /// <summary>
    /// Time when the undoable action was added.
    /// </summary>
    public readonly float AddedAt;

    public UndoableAction(Action perform, Action undo, float addedAt)
    {
        _do = perform;
        _undo = undo;

        AddedAt = addedAt;
    }

    public void Perform() => _do.Invoke();

    public void Undo() => _undo.Invoke();

    public void Append(UndoableAction otherAction)
    {
        _do += otherAction.Perform;
        _undo += otherAction.Undo;
    }
}
