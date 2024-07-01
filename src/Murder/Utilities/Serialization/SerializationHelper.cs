using Bang;
using Murder.Attributes;
using Murder.Diagnostics;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Murder.Utilities;

public static class SerializationHelper
{
    /// <summary>
    /// This is really tricky. We currently use readonly structs everywhere, which is great
    /// and very optimized.
    /// HOWEVER, this is tricky in the editor serializer. We need an actual new copy
    /// so we don't modify any other IComponents which are using the same memory.
    /// In order to workaround that, we will literally serialize a new component and create its copy.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code.", Justification = "Assembly is not trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "We use source generators.")]
    public static T DeepCopy<T>(T c)
    {
        GameLogger.Verify(c is not null);

        if (JsonSerializer.Deserialize<T>(
                JsonSerializer.Serialize(c, Game.Data.SerializationOptions), Game.Data.SerializationOptions) is not T obj)
        {
            throw new InvalidOperationException($"Unable to serialize {c.GetType().Name} for editor!?");
        }

        return obj;
    }

    public static IJsonTypeInfoResolver WithModifiers(this IJsonTypeInfoResolver resolver, params Action<JsonTypeInfo>[] modifiers)
        => new ModifierResolver(resolver, modifiers);

    private sealed class ModifierResolver : IJsonTypeInfoResolver
    {
        private readonly IJsonTypeInfoResolver _source;
        private readonly Action<JsonTypeInfo>[] _modifiers;

        public ModifierResolver(IJsonTypeInfoResolver source, Action<JsonTypeInfo>[] modifiers)
        {
            _source = source;
            _modifiers = modifiers;
        }

        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo? typeInfo = _source.GetTypeInfo(type, options);
            if (typeInfo != null)
            {
                foreach (Action<JsonTypeInfo> modifier in _modifiers)
                {
                    modifier(typeInfo);
                }
            }

            return typeInfo;
        }
    }

    internal static readonly ConcurrentDictionary<Type, List<JsonPropertyInfo>?> _types = [];

    [UnconditionalSuppressMessage("Trimming", "IL2026:CreateJsonPropertyInfo might be unable to create the appropriate instance.", Justification = "Not trimming dependent assemblies.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:CreateJsonPropertyInfo might be unable to create the appropriate instance.", Justification = "We are using source generators as we can.")]
    [UnconditionalSuppressMessage("AOT", "IL2075:Calling non-public fields with reflection.", Justification = "Assemblies are not trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL2072:Calling public constructors.", Justification = "Assemblies are not trimmed.")]
    public static void ApplyModifierForPrivateFieldsAndGetters(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        Type? t = jsonTypeInfo.Type;
        if (t.Assembly.FullName is null || t.Assembly.FullName.StartsWith("System"))
        {
            // Ignore system types.
            return;
        }

        HashSet<string> existingProperties = new(jsonTypeInfo.Properties.Count, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < jsonTypeInfo.Properties.Count; ++i)
        {
            JsonPropertyInfo property = jsonTypeInfo.Properties[i];

            bool shouldRemoveProperty = ShouldRemoveProperty(property, t);
            if (shouldRemoveProperty)
            {
                jsonTypeInfo.Properties.RemoveAt(i);
                --i;
            }
            else
            {
                existingProperties.Add(property.Name);
            }
        }

        // Keep track of added properties, so we don't add it twice between parent types.
        HashSet<string>? allExtraProperties = null;

        while (t is not null && t.Assembly.FullName is string name && !name.StartsWith("System"))
        {
            if (_types.TryGetValue(t, out var extraFieldsInParentType))
            {
                if (extraFieldsInParentType is not null)
                {
                    foreach (JsonPropertyInfo info in extraFieldsInParentType)
                    {
                        if (!existingProperties.Contains(info.Name))
                        {
                            JsonPropertyInfo infoForThisType = jsonTypeInfo.CreateJsonPropertyInfo(info.PropertyType, info.Name);

                            infoForThisType.Get = info.Get;
                            infoForThisType.Set = info.Set;

                            jsonTypeInfo.Properties.Add(infoForThisType);

                            existingProperties.Add(infoForThisType.Name);
                        }
                    }
                }
            }
            else
            {
                bool fetchedConstructors = false;
                HashSet<string>? parameters = null;

                // Slightly evil in progress code. If the field is *not* found as any of the constructor parameters,
                // manually use the setter via reflection. I don't care.
                for (int i = 0; i < jsonTypeInfo.Properties.Count; i++)
                {
                    JsonPropertyInfo info = jsonTypeInfo.Properties[i];

                    if (info.Set is not null)
                    {
                        continue;
                    }

                    if (!fetchedConstructors)
                    {
                        parameters = FetchConstructorParameters(t);
                    }

                    if (parameters is null || !parameters.Contains(info.Name))
                    {
                        FieldInfo? field = t.GetField(info.Name);
                        if (field is not null)
                        {
                            info.Set = field.SetValue;
                        }
                    }
                }

                List<JsonPropertyInfo>? extraPrivateProperties = null;

                // Now, this is okay. There is not much to do here. If the field is private, manually fallback to reflection.
                foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (!Attribute.IsDefined(field, typeof(SerializeAttribute)) && !Attribute.IsDefined(field, typeof(ShowInEditorAttribute)))
                    {
                        continue;
                    }

                    string fieldName = field.Name;

                    // We may need to manually format names for private fields.
                    if (fieldName.StartsWith('_'))
                    {
                        string nameWithoutUnderscore = fieldName[1..];

                        if (allExtraProperties is not null && allExtraProperties.Contains(nameWithoutUnderscore))
                        {
                            // This means that another derived type already added this field.
                            continue;
                        }

                        if (!existingProperties.Contains(nameWithoutUnderscore))
                        {
                            // Only modify the name if there is no conflicting existing field.
                            fieldName = nameWithoutUnderscore;
                        }
                    }

                    if (existingProperties.Contains(fieldName))
                    {
                        // This means that another derived type already added this field.
                        continue;
                    }

                    JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(field.FieldType, fieldName);

                    jsonPropertyInfo.Get = field.GetValue;
                    jsonPropertyInfo.Set = field.SetValue;

                    jsonTypeInfo.Properties.Add(jsonPropertyInfo);

                    extraPrivateProperties ??= [];
                    extraPrivateProperties.Add(jsonPropertyInfo);

                    allExtraProperties ??= [];
                    allExtraProperties.Add(fieldName);

                    existingProperties.Add(jsonPropertyInfo.Name);
                }

                _types[t] = extraPrivateProperties;
            }

            t = t.BaseType;
        }
    }

    [UnconditionalSuppressMessage("AOT", "IL2075:Calling non-public fields with reflection.", Justification = "Assemblies are not trimmed.")]
    private static bool ShouldRemoveProperty(JsonPropertyInfo property, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type t)
    {
        if (property.ShouldSerialize is not null)
        {
            // This means that are already rules in place that will likely deal with this serialization.
            return false;
        }

        if (property.Set is not null)
        {
            // Setter is available! Don't bother.
            return false;
        }

        if (t.GetProperty(property.Name) is not PropertyInfo prop)
        {
            // Fields are okay!
            return false;
        }

        if (prop.SetMethod is not null)
        {
            property.Set = prop.SetValue;
            return false;
        }

        // Now, there's a chance that the property is actually declared in a parent type with a private setter.
        // For such cases, let's manually check if the parent type has a setter.
        if (prop.DeclaringType is Type p && p.Assembly.FullName is string name && !name.StartsWith("System"))
        {
            if (p.GetProperty(property.Name) is PropertyInfo parentProperty && parentProperty.SetMethod is not null)
            {
                property.Set = parentProperty.SetValue;
                return false;
            }
        }
        
        // Skip readonly properties. Apparently System.Text.Json likes to ignore ReadOnlyProperties=false when applying to collections
        // so we will manually ignore them here.
        // These won't have a setter and that's why we reached this point.
        return true;
    }

    private static HashSet<string>? FetchConstructorParameters([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type t)
    {
        Type tAttribute = typeof(JsonConstructorAttribute);
        ConstructorInfo[] constructors = t.GetConstructors();

        foreach (ConstructorInfo info in constructors)
        {
            if (!Attribute.IsDefined(info, tAttribute))
            {
                continue;
            }

            HashSet<string> result = new(StringComparer.OrdinalIgnoreCase);

            foreach (ParameterInfo parameter in info.GetParameters())
            {
                if (parameter.Name is null)
                {
                    continue;
                }

                result.Add(parameter.Name);
            }

            return result;
        }

        return null;
    }
}