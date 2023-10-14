using Murder.Core.Dialogs;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<DialogAction>))]
    internal class ImmutableDialogActionField : ImmutableArrayField<DialogAction>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out DialogAction element)
        {
            element = default;

            if (ImGuiHelpers.IconButton('\uf055', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                element = new();
                return true;
            }

            return false;
        }

        protected override bool DrawElement(ref DialogAction element, EditorMember member, int index)
        {
            (bool modified, DialogAction newValue) = DialogActionField.DrawActionEditor(element);
            if (modified)
            {
                element = newValue;
                return true;
            }

            return false;
        }
    }
}