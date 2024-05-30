namespace Murder.Editor.Core;

public interface IUndoableAction
{
    void Perform();

    void Undo();
}

public class UndoTracker
{
    private readonly Stack<IUndoableAction> _undo;
    private readonly Stack<IUndoableAction> _redo;

    public UndoTracker(int capacity)
    {
        _undo = new(capacity);
        _redo = new(capacity);
    }

    public void Act(IUndoableAction action)
    {
        _undo.Push(action);
        _redo.Clear();

        action.Perform();
    }

    public void Undo()
    {
        if (_undo.Count == 0)
        {
            return;
        }

        IUndoableAction action = _undo.Pop();
        action.Undo();

        _redo.Push(action);
    }

    public void Redo()
    {
        if (_redo.Count == 0)
        {
            return;
        }

        IUndoableAction action = _redo.Pop();
        action.Perform();

        _undo.Push(action);
    }
}
