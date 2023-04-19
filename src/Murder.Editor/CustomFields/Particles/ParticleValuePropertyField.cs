using ImGuiNET;
using Murder.Attributes;
using Murder.Core.Particles;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ParticleValueProperty))]
    internal class ParticleValuePropertyField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            ParticleValueProperty value = (ParticleValueProperty)fieldValue!;

            using TableMultipleColumns table = new($"value_property", flags: ImGuiTableFlags.SizingFixedFit, 
                -1, 400);

            ImGui.TableNextColumn();
            ImGui.TableNextColumn();

            ImGui.PushItemWidth(-1);

            ImGuiHelpers.ColorIcon(value.Kind == ParticleValuePropertyKind.Constant ?
                '\uf528' : '\uf522', Game.Profile.Theme.Faded);
            ImGui.SameLine();

            if (member.Member.DeclaringType == typeof(Particle))
            {
                ImGuiHelpers.HelpTooltip("Range of values of the partcile over its lifetime.");
            }
            else
            {
                ImGuiHelpers.HelpTooltip("How is this variable defined when creating a new particle.");
            }


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
                    modified |= DrawConstant(member, ref value);
                    break;
                    
                case ParticleValuePropertyKind.Range:
                    modified |= DrawRange(member, ref value);
                    break;

                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    modified |= DrawRangedRange(member, ref value);
                    break;

                case ParticleValuePropertyKind.Curve:
                    modified |= DrawCurve(member, ref value);
                    break;
            }

            ImGui.PopItemWidth();
            return (modified, value);
        }

        public static bool DrawCurve<T>(EditorMember member, ref T value)
        {
            AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider);
            
            return ImmutableArrayFloatField.DrawValue(ref value, "_curvePoints");
        }

        public static bool DrawConstant<T>(EditorMember member, ref T value)
        {
            if (AttributeExtensions.IsDefined(member, typeof(AngleAttribute)))
            {
                return DrawPrimitiveAngle($"{member.Name}_constant", ref value, "_constant");
            }
            
            AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider);
            return DrawPrimitiveValueWithSlider(id: $"{member.Name}_constant", ref value, "_constant", slider);
        }
        
        public static bool DrawRange<T>(EditorMember member, ref T value)
        {
            bool modified = false;
            bool hasAngle = AttributeExtensions.IsDefined(member, typeof(AngleAttribute));

            AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider);

            ImGui.Text("From");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);
            
            modified |= hasAngle ?
                DrawPrimitiveAngle($"{member.Name}_rangestart", ref value, "_rangeStart") :
                DrawPrimitiveValueWithSlider(id: $"{member.Name}_rangestart", ref value, "_rangeStart", slider);
            
            ImGui.SameLine();

            ImGui.PopItemWidth();
            
            ImGui.Text("To");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);
            
            modified |= hasAngle ? 
                DrawPrimitiveAngle($"{member.Name}_rangeend", ref value, "_rangeEnd") :
                DrawPrimitiveValueWithSlider(id: $"{member.Name}_rangeend", ref value, "_rangeEnd", slider);

            ImGui.PopItemWidth();

            return modified;
        }
        
        public static bool DrawRangedRange<T>(EditorMember member, ref T value)
        {
            bool modified = false;
            bool hasAngle = AttributeExtensions.IsDefined(member, typeof(AngleAttribute));
            
            AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider);

            ImGui.Text("From");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);

            modified |= hasAngle ?
                DrawPrimitiveAngle($"{member.Name}_rangestartmin", ref value, "_rangeStartMin") : 
                DrawPrimitiveValueWithSlider($"{member.Name}_rangestartmin", ref value, "_rangeStartMin", slider);
            ImGui.SameLine();

            ImGui.PopItemWidth();

            ImGui.Text("To");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);

            modified |= hasAngle ?
                DrawPrimitiveAngle($"{member.Name}_rangestartmax", ref value, "_rangeStartMax") : 
                DrawPrimitiveValueWithSlider($"{member.Name}_rangestartmax", ref value, "_rangeStartMax", slider);

            ImGui.PopItemWidth();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TableNextColumn();

            ImGui.Text("From");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);

            modified |= hasAngle ?
                DrawPrimitiveAngle($"{member.Name}_rangeendmin", ref value, "_rangeEndMin") :
                DrawPrimitiveValueWithSlider($"{member.Name}_rangeendmin", ref value, "_rangeEndMin", slider);
            ImGui.SameLine();

            ImGui.PopItemWidth();

            ImGui.Text("To");
            ImGui.SameLine();
            
            ImGui.PushItemWidth(100);

            modified |= hasAngle ?
                DrawPrimitiveAngle($"{member.Name}_rangeendmax", ref value, "_rangeEndMax") :
                DrawPrimitiveValueWithSlider($"{member.Name}_rangeendmax", ref value, "_rangeEndMax", slider);

            ImGui.PopItemWidth();
            
            return modified;
        }
    }
}
