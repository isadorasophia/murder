using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(DialogAction))]
    internal class DialogActionField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            return DrawActionEditor(fieldValue);
        }

        public static (bool modified, DialogAction result) DrawActionEditor(object? fieldValue)
        {
            bool modified = false;

            DialogAction action = (DialogAction)fieldValue!;


            return (modified, action);
        }
    }
}
