namespace Murder.Editor;

public interface IUndoableAction
{
    void Perform();
    void Undo();
}

public sealed class SimpleUndoableAction(
    Action perform,
    Action undo
) : IUndoableAction
{
    public void Perform()
    {
        perform();
    }

    public void Undo()
    {
        undo();
    }
}

public sealed class BooleanUndoableAction(
    bool newValue,
    Action<bool> action
) : IUndoableAction
{
    public void Perform()
    {
        action(newValue);
    }

    public void Undo()
    {
        action(!newValue);
    }
}

public sealed class IntegerUndoableAction(
    int currentValue,
    int newValue,
    Action<int> action
) : IUndoableAction
{
    public void Perform()
    {
        action(currentValue);
    }

    public void Undo()
    {
        action(newValue);
    }
}