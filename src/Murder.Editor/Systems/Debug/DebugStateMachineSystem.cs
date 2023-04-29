using Bang.Systems;
using System.Collections.Immutable;
using Murder.Systems;
using Bang;
using Bang.Entities;
using Bang.StateMachines;
using System.Reflection;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This is a system used to debug state machine components, by calling the OnStart() method
    /// on the editor.
    /// </summary>
    [DoNotPause]
    [Filter(typeof(IStateMachineComponent))]
    [Watch(typeof(IStateMachineComponent))]
    public class DebugStateMachineSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                CallStartMethod(e.GetStateMachine());
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                CallStartMethod(e.GetStateMachine());
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }
        
        private static MethodInfo? _onStartMethod;

        private static FieldInfo GetRoutineField(Type tGenericStateMachine)
        {
            FieldInfo? tRoutineField = tGenericStateMachine.GetField("_routine", BindingFlags.NonPublic | BindingFlags.Instance);
            return tRoutineField!;
        }
        
        private static MethodInfo OnStartMethod()
        {
            Type tStateMachcine = typeof(StateMachine);
            MethodInfo? onStartMethod = tStateMachcine.GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance);

            return onStartMethod!;
        }

        /// <summary>
        /// Horrifying method which will call start on a state machine through
        /// ~*~reflection~*~.
        /// This only happens because: I refuse to make this method public.
        /// </summary>
        public static void CallStartMethod(IStateMachineComponent stateMachine)
        {
            _onStartMethod ??= OnStartMethod();
            
            FieldInfo field = GetRoutineField(stateMachine.GetType());
            StateMachine? routine = field.GetValue(stateMachine) as StateMachine;

            _onStartMethod?.Invoke(routine, parameters: Array.Empty<object>());
        }
    }
}
