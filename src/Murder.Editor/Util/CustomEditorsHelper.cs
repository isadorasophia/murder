using InstallWizard.DebugUtilities;
using Editor.CustomComponents;
using Editor.CustomEditors;
using Editor.CustomFields;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Editor.Util
{
    internal static class CustomEditorsHelper
    {
        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, CustomComponent CustomComponent)>> _customComponentEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, CustomComponent)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefined<CustomComponentOfAttribute>(ofType: typeof(CustomComponent)))
            {
                var attribute = Attribute.GetCustomAttribute(t, typeof(CustomComponentOfAttribute)) as CustomComponentOfAttribute;
                var customField = Activator.CreateInstance(t) as CustomComponent;

                GameLogger.Verify(customField != null);
                GameLogger.Verify(attribute != null);

                builder.Add(attribute.OfType, (attribute.Priority, customField));
            }

            return builder.ToImmutable();
        });

        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, CustomField CustomField)>> _customFieldEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, CustomField)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefined<CustomFieldOfAttribute>(ofType: typeof(CustomField)))
            {
                var attribute = Attribute.GetCustomAttribute(t, typeof(CustomFieldOfAttribute)) as CustomFieldOfAttribute;
                var customField = Activator.CreateInstance(t) as CustomField;

                GameLogger.Verify(customField != null);
                GameLogger.Verify(attribute != null);

                builder.Add(attribute.OfType, (attribute.Priority, customField));
            }

            return builder.ToImmutable();
        });

        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, CustomEditor CustomEditor)>> _customEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, CustomEditor)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefined<CustomEditorOfAttribute>())
            {
                var attribute = Attribute.GetCustomAttribute(t, typeof(CustomEditorOfAttribute)) as CustomEditorOfAttribute;
                var customEditor = Activator.CreateInstance(t) as CustomEditor;

                GameLogger.Verify(customEditor != null);
                GameLogger.Verify(attribute != null);

                builder.Add(attribute.OfType, (attribute.Priority, customEditor));
            }

            return builder.ToImmutable();
        });

        public static bool TryGetCustomFieldEditor(Type t, [NotNullWhen(true)] out CustomField? f)
        {
            if (_customFieldEditors.Value.TryGetValue(t, out var result))
            {
                f = result.CustomField;
                return true;
            }

            var allMatchers = _customFieldEditors.Value.Where(kv => kv.Key.IsAssignableFrom(t));
            if (allMatchers.Any())
            {
                f = allMatchers.OrderByDescending(kv => kv.Value.Priority).First().Value.CustomField;
                return true;
            }

            f = default;
            return false;
        }

        public static bool TryGetCustomEditor(Type t, [NotNullWhen(true)] out CustomEditor? f)
        {
            if (_customEditors.Value.TryGetValue(t, out var result))
            {
                f = result.CustomEditor;
                return true;
            }

            var allMatchers = _customEditors.Value.Where(kv => kv.Key.IsAssignableFrom(t));
            if (allMatchers.Any())
            {
                f = allMatchers.OrderByDescending(kv => kv.Value.Priority).First().Value.CustomEditor;
                return true;
            }

            f = default;
            return false;
        }

        public static bool TryGetCustomComponent(Type t, [NotNullWhen(true)] out CustomComponent? c)
        {
            if (_customComponentEditors.Value.TryGetValue(t, out var result))
            {
                c = result.CustomComponent;
                return true;
            }

            var allMatchers = _customComponentEditors.Value.Where(kv => kv.Key.IsAssignableFrom(t));
            if (allMatchers.Any())
            {
                c = allMatchers.OrderByDescending(kv => kv.Value.Priority).First().Value.CustomComponent;
                return true;
            }

            c = default;
            return false;
        }
    }
}
