using ImGuiNET;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Murder.Editor.CustomFields
{
    public abstract class CustomField
    {
        public abstract (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue);

        public static bool DrawValueWithId<T>(ref T target, string fieldName)
        {
            ImGui.PushItemWidth(-1);
            ImGui.PushID(fieldName);
            bool result = DrawValue(ref target, fieldName);
            ImGui.PopID();
            ImGui.PopItemWidth();

            return result;
        }

        public static bool DrawValue<T>(ref T target, string fieldName)
        {
            if (target is null ||
                target.GetType().TryGetFieldForEditor(fieldName) is not EditorMember member)
            {
                return false;
            }

            (bool modified, object? boxedResult) = DrawValue(member, member.GetValue(target));
            if (modified)
            {
                member.SetValue(ref target, boxedResult);
                return true;
            }

            return false;
        }

        public static bool DrawValue<T>(EditorMember member, T input, [NotNullWhen(true)] out T? result)
        {
            (bool modified, object? boxedResult) = DrawValue(member, input);

            result = (T?)boxedResult;
            return modified;
        }

        public static (bool Modified, object? Result) DrawValue(EditorMember member, /* ref */ object? value)
        {
            bool modified = false;
            object? result = value;

            if (value is not null)
            {
                if (Nullable.GetUnderlyingType(member.Type) != null)
                {
                    bool delete = ImGuiHelpers.DeleteButton($"##{member.Name}_delete");
                    ImGuiHelpers.HelpTooltip("Restore default value");

                    ImGui.SameLine();

                    if (delete)
                    {
                        return (true, null);
                    }
                }
            }

            if (CustomEditorsHelper.TryGetCustomFieldEditor(member.Type, out var customFieldEditor))
            {
                return customFieldEditor.ProcessInput(member, /* ref */ value);
            }

            // Check for non-nullable types
            switch (value)
            {
                case float number:
                    return (DrawFloat(member, ref number), number);

                case double number:
                    float toFloat = (float)number;
                    return (DrawFloat(member, ref toFloat), toFloat);

                case bool flag:
                    return member.IsReadOnly ? DrawReadOnly(flag) : 
                        (ImGui.Checkbox("", ref flag), flag);
                    
                case object obj:
                    Type? t = value.GetType();
                    if (t!.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // This is actually a nullable, so wrap this value so the user has the opportunity to create
                        // a default value.
                        if (ImGui.TreeNode($"({member.Name})"))
                        {
                            ImGui.SameLine();
                            if (ImGuiHelpers.DeleteButton($"del_{member.Name}"))
                            {
                                if (t!.GetConstructor(Type.EmptyTypes) != null)
                                {
                                    var defaultValue = Activator.CreateInstance(t);
                                    if (defaultValue != null)
                                    {
                                        modified = true;
                                        result = defaultValue;
                                    }
                                }
                                else
                                {
                                    var defaultValue = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(t);
                                    if (defaultValue != null)
                                    {
                                        modified = true;
                                        result = defaultValue;
                                    }
                                }

                                return (modified, result);
                            }

                            (modified, result) = (CustomComponent.ShowEditorOf(obj), obj);
                            ImGui.TreePop();

                            return (modified, result);
                        }
                    }
                    else
                    {
                        if (CustomEditorsHelper.TryGetCustomFieldEditor(t, out customFieldEditor))
                        {
                            return customFieldEditor.ProcessInput(member, /* ref */ value);
                        }
                        
                        (modified, result) = (CustomComponent.ShowEditorOf(obj), obj);
                    }
                    
                    break;

                case null:
                    string text = "Create Default";
                    if (AttributeExtensions.TryGetAttribute(member, out DefaultAttribute? defaultAttribute))
                    {
                        text = defaultAttribute.Text;
                    }

                    // Check for nullable types. If so, return the default value of it.
                    Type targetType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

                    if (ImGui.Button(text))
                    {
                        if (member.Type == typeof(string))
                        {
                            return (true, string.Empty);
                        }

                        try
                        {
                            if (targetType.IsInterface)
                            {
                                ImGui.OpenPopup($"create_{targetType.Name}");
                            }
                            else
                            {
                                return (true, Activator.CreateInstance(targetType));
                            }
                        }
                        catch (MissingMethodException)
                        {
                            GameLogger.Error(
                                $"Unable to find default constructor for type: {member.Type.Name}.");
                        }
                    }

                    // When this is an interface, select the most appropriate instance.
                    (bool modified, object? result)? selectedType = DrawCreateDefaultValue(targetType);
                    if (selectedType is not null)
                    {
                        return selectedType.Value;
                    }

                    return (false, default);
            }

            return (modified, result);
        }

        /// <summary>
        /// Create a default value for a field with an interface.
        /// </summary>
        private static (bool Modified, object? Result)? DrawCreateDefaultValue(Type interfaceType)
        {
            (bool modified, object? result)? result = null;

            if (ImGui.BeginPopup($"create_{interfaceType.Name}"))
            {
                ImGui.Text($"Select instance of {interfaceType.Name}:");

                ImGui.BeginChild("search_interface", new System.Numerics.Vector2(240, 20));
                ImGui.PushItemWidth(300);

                if (SearchBox.SearchInterfaces(interfaceType) is Type tTarget)
                {
                    result = (true, Activator.CreateInstance(tTarget));
                }

                ImGui.PopItemWidth();
                ImGui.EndChild();

                ImGui.EndPopup();
            }

            return result;
        }

        private static bool DrawFloat(EditorMember member, ref float number)
        {
            if (AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider))
            {
                return ImGui.SliderFloat("", ref number, slider.Minimum, slider.Maximum);
            }

            if (member.IsReadOnly)
            {
                DrawReadOnly(number);
                return false;
            }

            return ImGui.InputFloat("", ref number, 1);
        }

        public static bool DrawPrimitiveAngle<T>(string id, ref T target, string fieldName)
        {
            if (target is null ||
                target.GetType().TryGetFieldForEditor(fieldName) is not EditorMember member)
            {
                return false;
            }

            bool modified = false;
            object? value = member.GetValue(target);

            switch (value)
            {
                case float number:
                    number *= Calculator.TO_DEG;
                    modified |= ImGui.SliderFloat($"##{id}", ref number, 0, 360);
                    value = number * Calculator.TO_RAD;
                    break;

                case int iNumber:
                    iNumber = Calculator.RoundToInt(iNumber*Calculator.TO_DEG);
                    modified |= ImGui.SliderInt($"##{id}", ref iNumber, 0, 360);
                    value = Calculator.RoundToInt(iNumber * Calculator.TO_RAD);
                    break;
            }

            if (modified)
            {
                member.SetValue(ref target, value);
                return true;
            }

            return false;
        }
        
        public static bool DrawPrimitiveValueWithSlider<T>(
            string id, ref T target, string fieldName, SliderAttribute? slider)
        {
            if (target is null ||
                target.GetType().TryGetFieldForEditor(fieldName) is not EditorMember member)
            {
                return false;
            }

            bool modified = false;
            object? value = member.GetValue(target);

            switch (value)
            {
                case float number:
                    if (slider is not null)
                    {
                        modified |= ImGui.SliderFloat($"##{id}", ref number, slider.Minimum, slider.Maximum);
                    }
                    else
                    {
                        modified |= ImGui.InputFloat($"##{id}", ref number, 1);
                    }

                    value = number;
                    break;

                case int iNumber:
                    if (slider is not null)
                    {
                        modified |= ImGui.SliderInt(
                            $"##{id}", ref iNumber, 
                            Calculator.RoundToInt(slider.Minimum), Calculator.RoundToInt(slider.Maximum));
                    }
                    else
                    {
                        modified |= ImGui.InputInt($"##{id}", ref iNumber, 1);
                    }

                    value = iNumber;
                    break;
                    
                case Core.Geometry.Vector2 vector2Core:
                    System.Numerics.Vector2 vec = new (vector2Core.X, vector2Core.Y);
                    if (slider is not null)
                    {
                        modified |= ImGui.SliderFloat2($"##{id}", ref vec, slider.Minimum, slider.Maximum);
                    }
                    else
                    {
                        modified |= ImGui.InputFloat2($"##{id}", ref vec);
                    }

                    value = new Core.Geometry.Vector2(vec.X, vec.Y);
                    break;

            }

            if (modified)
            {
                member.SetValue(ref target, value);
                return true;
            }

            return false;
        }

        protected static (bool Modified, object? Result) DrawReadOnly(object? value)
        {
            ImGui.Text(value?.ToString() ?? "null");
            ImGuiHelpers.HelpTooltip("Read only");

            return (false, value);
        }
    }
}
