using ImGuiNET;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.ImGuiExtended;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    public abstract class CustomField
    {
        public abstract (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue);

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

            if (CustomEditorsHelper.TryGetCustomFieldEditor(member.Type, out var customFieldEditor))
            {
                return customFieldEditor.ProcessInput(member, /* ref */ value);
            }

            // Check for non-nullable types
            switch (value)
            {
                case int number:
                    return (ImGui.InputInt("", ref number, 1), number);

                case float number:
                    if (AttributeExtensions.TryGetAttribute(member, out SliderAttribute? slider))
                    {
                        return (ImGui.SliderFloat("", ref number, slider.Minimum, slider.Maximum), number);
                    }
                    else
                    {
                        return (ImGui.InputFloat("", ref number, 1), number);
                    }

                case bool flag:
                    return (ImGui.Checkbox("", ref flag), flag);

                case Guid guid:
                    if (AttributeExtensions.TryGetAttribute(member, out GameAssetIdAttribute? gameAssetAttr))
                    {
                        var guidValue = guid;
                        var changed = SearchBox.SearchAsset(ref guidValue, gameAssetAttr.AssetType);
                        return (changed, guidValue);
                    }

                    break;

                case object obj:
                    var t = obj.GetType();
                    if (ImGui.TreeNode($"({member.Name})"))
                    {
                        ImGui.SameLine();
                        if (ImGuiHelpers.DeleteButton($"del_{member.Name}"))
                        {
                            if (t.GetConstructor(Type.EmptyTypes) != null)
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
                        }

                        (modified, result) = (CustomComponent.ShowEditorOf(obj), obj);
                        ImGui.TreePop();

                        return (modified, result);
                    }

                    break;

                case null:
                    ImGui.TextColored(Game.Profile.Theme.Faded, $" NULL {member.Type.Name}");
                    ImGui.SameLine();

                    if (ImGui.Button("Create Default"))
                    {
                        if (member.Type == typeof(string))
                        {
                            return (true, string.Empty);
                        }

                        // Check for nullable types. If so, return the default value of it.
                        Type targetType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

                        try
                        {
                            return (true, Activator.CreateInstance(targetType));
                        }
                        catch (MissingMethodException)
                        {
                            GameLogger.Error(
                                $"Unable to find default constructor for type: {member.Type.Name}.");
                        }
                    }

                    return (false, default);
            }

            return (modified, result);
        }

    }
}
