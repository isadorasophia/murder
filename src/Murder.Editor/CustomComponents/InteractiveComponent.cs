using InstallWizard.DebugUtilities;
using Editor.Reflection;
using Editor.Util;
using Bang.Interactions;

namespace Editor.CustomComponents
{
    [CustomComponentOf(typeof(IInteractiveComponent), priority: 10)]
    public class InteractiveComponent : CustomComponent
    {
        private EditorMember GetInteractionField(Type t) =>
            ReflectionHelper.TryGetFieldForEditor(t, "_interaction")!;
        
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            EditorMember interactionMember = GetInteractionField(target.GetType());
            
            object? interactionObj = interactionMember.GetValue(target);
            if (interactionObj is null)
            {
                GameLogger.Fail("Why is the value of the interaction null?");
                return false;
            }

            return ProcessInput(target, interactionMember, () => (DrawInteraction(interactionObj), interactionObj));
        }

        protected virtual bool DrawInteraction(object? interaction)
        {
            return ShowEditorOf(interaction);
        }
    }
}
