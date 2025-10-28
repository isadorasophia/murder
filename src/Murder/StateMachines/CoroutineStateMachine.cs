using Bang.StateMachines;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.StateMachines;

/// <summary>
/// This CANNOT and WONT be serialized it is just a bad idea. Remember, we can't (or don't want to) serialize lambdas.
/// </summary>
[RuntimeOnly]
public class CoroutineStateMachine : StateMachine
{
    private readonly IEnumerator<Wait> _routine;

    public CoroutineStateMachine() : this(None()) { }

    public CoroutineStateMachine(IEnumerator<Wait> routine)
    {
        _routine = routine;

        State(Run);
    }

    private IEnumerator<Wait> Run()
    {
        yield return Wait.ForRoutine(_routine);
    }

    /// <summary>
    /// This is called if this was created without a routine, for whatever reason (it shouldn't).
    /// </summary>
    private static IEnumerator<Wait> None()
    {
        yield return Wait.Stop;
    }
}