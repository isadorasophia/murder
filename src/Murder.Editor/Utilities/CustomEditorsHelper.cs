using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.CustomEditors;
using Murder.Editor.CustomFields;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.Utilities
{
    internal static class CustomEditorsHelper
    {
        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, CustomComponent CustomComponent)>> _customComponentEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, CustomComponent)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<CustomComponentOfAttribute>(ofType: typeof(CustomComponent)))
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
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<CustomFieldOfAttribute>(ofType: typeof(CustomField)))
            {
                if (t.IsGenericTypeDefinition)
                {
                    // Skip generic type definitions
                    continue;
                }
                
                CustomFieldOfAttribute? attribute = Attribute.GetCustomAttribute(t, typeof(CustomFieldOfAttribute)) as CustomFieldOfAttribute;
                CustomField? customField = Activator.CreateInstance(t) as CustomField;

                GameLogger.Verify(customField != null);
                GameLogger.Verify(attribute != null);

                builder.Add(attribute.OfType, (attribute.Priority, customField));
            }
            
            return builder.ToImmutable();
        });

        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, Type CustomFieldType)>> _customGenericFieldEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, Type)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<CustomFieldOfAttribute>(ofType: typeof(CustomField)))
            {
                if (!t.IsGenericTypeDefinition)
                {
                    // This only handles generic field editors.
                    continue;
                }

                CustomFieldOfAttribute? attribute = Attribute.GetCustomAttribute(t, typeof(CustomFieldOfAttribute)) as CustomFieldOfAttribute;
                GameLogger.Verify(attribute != null);

                builder.Add(attribute.OfType, (attribute.Priority, t));
            }

            return builder.ToImmutable();
        });

        private static readonly Dictionary<Type, CustomField> _cachedGenericFieldEditors = new();

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

            // Look for generic fields.
            if (t.IsGenericType)
            {
                if (_cachedGenericFieldEditors.TryGetValue(t, out f))
                {
                    return true;
                }
                
                if (_customGenericFieldEditors.Value.TryGetValue(t.GetGenericTypeDefinition(), out var customField))
                {
                    f = Activator.CreateInstance(customField.CustomFieldType.MakeGenericType(t.GetGenericArguments())) as CustomField;
                    if (f is not null)
                    {
                        _cachedGenericFieldEditors.Add(t, f);
                    }

                    return f is not null;
                }
            }
            
            f = default;
            return false;
        }

        /// <summary>
        /// Returns all custom editors with their priority.
        /// </summary>
        public static readonly Lazy<ImmutableDictionary<Type, (int Priority, Type tCustomEditor)>> _customEditors = new(() =>
        {
            var builder = ImmutableDictionary.CreateBuilder<Type, (int, Type)>();
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefined<CustomEditorOfAttribute>())
            {
                CustomEditorOfAttribute attribute = 
                    (CustomEditorOfAttribute)Attribute.GetCustomAttribute(t, typeof(CustomEditorOfAttribute))!;

                builder.Add(attribute.OfType, (attribute.Priority, t));
            }

            return builder.ToImmutable();
        });

        /// <summary>
        /// Returns a CustomEditor corresponding to the editor that targets <paramref name="t"/>.
        /// </summary>
        public static bool TryGetCustomEditor(Type t, [NotNullWhen(true)] out Type? f)
        {
            if (_customEditors.Value.TryGetValue(t, out var result))
            {
                f = result.tCustomEditor;
                return true;
            }

            var allMatchers = _customEditors.Value.Where(kv => kv.Key.IsAssignableFrom(t));
            if (allMatchers.Any())
            {
                f = allMatchers.OrderByDescending(kv => kv.Value.Priority).First().Value.tCustomEditor;
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
