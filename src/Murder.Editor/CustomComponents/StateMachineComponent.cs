using InstallWizard.DebugUtilities;
using Editor.Reflection;
using Editor.Util;
using ImGuiNET;
using Bang.StateMachines;

namespace Editor.CustomComponents
{
    [CustomComponentOf(typeof(IStateMachineComponent))]
    public class StateMachineComponent : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            var members = target.GetType().GetFieldsForEditor("State");
            EditorMember? stateMachineField = members.FirstOrDefault(m => m.Name == "_routine");
            if (stateMachineField is null)
            {
                GameLogger.Fail("Why is the state machine null?");
                return false;
            }

            object? stateMachine = stateMachineField.GetValue(target);
            if (stateMachine is null)
            {
                GameLogger.Fail("Why is the value of the interaction null?");
                return false;
            }

            string currentState;

            EditorMember persistedState = typeof(StateMachine).TryGetFieldForEditor("_cachedPersistedState")!;
            if (persistedState.GetValue(stateMachine) is string persistedStateValue)
            {
                currentState = persistedStateValue;
            }
            else
            {
                // TODO: Implement state for coroutines being waited on.
                // Or just make it prettier.
                EditorMember state = members.First(m => m.Name == "State");
                currentState = (string)state.GetValue(target)!;
            }

            ImGui.Text(currentState);

            return ProcessInput(target, stateMachineField, () => (ShowEditorOf(stateMachine), stateMachine));
        }
    }
}
