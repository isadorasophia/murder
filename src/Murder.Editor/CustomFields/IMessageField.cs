using Bang.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(IMessage), priority: -10)]
    internal class IMessageField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            IMessage? message = (IMessage?)fieldValue;
            if (member.Type.IsInterface)
            {
                if (SearchBox.SearchInterfaces(typeof(IMessage), message?.GetType()) is Type t)
                {
                    modified = true;
                    message = (IMessage)Activator.CreateInstance(t)!;
                }
            }

            if (message is not null)
            {
                modified |= CustomComponent.ShowEditorOf(ref message);
            }

            return (modified, message);
        }
    }
}
