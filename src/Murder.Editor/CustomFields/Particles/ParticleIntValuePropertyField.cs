using ImGuiNET;
using Murder.Attributes;
using Murder.Core.Particles;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ParticleIntValueProperty))]
    internal class ParticleIntValuePropertyField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            ParticleIntValueProperty value = (ParticleIntValueProperty)fieldValue!;

            using TableMultipleColumns table = new($"value_property", flags: ImGuiTableFlags.SizingFixedFit, 
                -1, 400);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGuiHelpers.ColorIcon(value.Kind == ParticleValuePropertyKind.Constant ? '\uf528' : '\uf522', Game.Profile.Theme.Faded);

            if (member.Member.DeclaringType == typeof(Particle))
            {
                ImGuiHelpers.HelpTooltip("Range of values of the particle over its lifetime.");
            }
            else
            {
                ImGuiHelpers.HelpTooltip("How is this variable defined when creating a new particle.");
            }

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            if (DrawValue(ref value, nameof(ParticleValueProperty.Kind)))
            {
                modified = true;
            }

            ImGui.PopItemWidth();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            switch (value.Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider);
                    modified |= DrawPrimitiveValueWithSlider(id: $"{member.Name}_constant", ref value, "_constant", slider);
                    break;
                    
                case ParticleValuePropertyKind.Range:
                    modified |= ParticleValuePropertyField.DrawRange(member, ref value);
                    break;
                    
                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    modified |= ParticleValuePropertyField.DrawRangedRange(member, ref value);
                    break;
            }

            ImGui.PopItemWidth();
            return (modified, value);
        }
    }
}
