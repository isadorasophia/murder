using Bang.StateMachines;
using ImGuiNET;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Reflection;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(IStateMachineComponent))]
    public class StateMachineComponent : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            bool modified = false;

            IEnumerable<EditorMember> members = target.GetType().GetFieldsForEditor("State");
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
                EditorMember state = members.First(m => m.Name == "State");
                currentState = (string)state.GetValue(target)!;
            }

            // Find all state candidates.
            Type? tStateMachine = target.GetType().GetGenericArguments().FirstOrDefault();
            if (tStateMachine is not null)
            {
                IEnumerable<string> states =
                    tStateMachine.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                 .Where(m => m.ReturnType.IsGenericType &&
                                        m.ReturnType.GetGenericTypeDefinition() == typeof(IEnumerator<>))
                                 .Select(m => m.Name);

                if (ImGui.BeginCombo("##state_machine", currentState))
                {
                    foreach (string s in states)
                    {
                        if (ImGui.MenuItem(s))
                        {
                            persistedState.SetValue(stateMachine, s);
                            modified = true;
                        }
                    }

                    ImGui.EndCombo();
                }
            }

            modified |= ProcessInput(target, stateMachineField, () => (ShowEditorOf(stateMachine), stateMachine));

            return modified;
        }
    }
}