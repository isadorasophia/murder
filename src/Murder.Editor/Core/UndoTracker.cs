namespace Murder.Editor.Core;

public class UndoTracker
{
    private readonly Stack<IUndoableAction> _undo;
    private readonly Stack<IUndoableAction> _redo;

    public UndoTracker(int capacity)
    {
        _undo = new(capacity);
        _redo = new(capacity);
    }

    public void Track(IUndoableAction action)
    {
        _undo.Push(action);
        _redo.Clear();
    }

    public void Track(Action @do, Action @undo) => Track(new UndoableAction(@do, @undo));

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

public interface IUndoableAction
{
    void Perform();

    void Undo();
}

public class UndoableAction : IUndoableAction
{
    private readonly Action _do;
    private readonly Action _undo;

    public UndoableAction(Action perform, Action undo)
    {
        _do = perform;
        _undo = undo;
    }

    public void Perform() => _do.Invoke();

    public void Undo() => _undo.Invoke();
}
