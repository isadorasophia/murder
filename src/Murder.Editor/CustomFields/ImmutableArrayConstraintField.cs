using System.Collections.Immutable;
using ImGuiNET;
using Murder.Core.Ui;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields;

[CustomFieldOf(typeof(ImmutableArray<IConstraint>))]
internal class ConstraintArrayField : ImmutableArrayField<IConstraint>
{
    protected override bool Add(in EditorMember member, out IConstraint? element)
    {
        element = null;
        ImGui.PushID($"Add ${member.Member.ReflectedType}");

        var result = false;
        if (SearchBox.SearchConstraints() is { } constraintType)
        {
            if (Activator.CreateInstance(constraintType) is IConstraint constraint)
            {
                element = constraint;
                result = true;
            }
        }

        ImGui.PopID();
        
        return result;
    }
}