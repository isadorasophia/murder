namespace Murder.Editor;

public static class UndoStack
{
    private static readonly Stack<IUndoableAction> _undoStack = new();
    private static readonly Stack<IUndoableAction> _redoStack = new();

    public static void PerformAction(IUndoableAction undoableAction)
    {
        _undoStack.Push(undoableAction);
        undoableAction.Perform();
        _redoStack.Clear();
    }

    public static void Undo()
    {
        if (_undoStack.Count <= 0)
            return;
            
        IUndoableAction actionToUndo = _undoStack.Pop();
        actionToUndo.Undo();
        
        _redoStack.Push(actionToUndo);
    }

    public static void Redo()
    {
        if (_redoStack.Count <= 0)
            return;

        IUndoableAction actionToRedo = _redoStack.Pop();
        actionToRedo.Perform();
        
        _undoStack.Push(actionToRedo);
    }
}