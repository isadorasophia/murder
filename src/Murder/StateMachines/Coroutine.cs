using Bang.StateMachines;
using Murder.Utilities.Attributes;

namespace Murder.StateMachines
{
    [RuntimeOnly]
    public class Coroutine : StateMachine
    {
        private readonly IEnumerator<Wait> _routine;

        public Coroutine() : this(None()) { }

        public Coroutine(IEnumerator<Wait> routine)
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
}