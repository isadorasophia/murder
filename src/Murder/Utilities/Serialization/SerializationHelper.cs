using Bang;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Murder.Utilities;

public static class SerializationHelper
{
    #region Legacy

    /// <summary>
    /// This is really tricky. We currently use readonly structs everywhere, which is great
    /// and very optimized.
    /// HOWEVER, this is tricky in the editor serializer. We need an actual new copy
    /// so we don't modify any other IComponents which are using the same memory.
    /// In order to workaround that, we will literally serialize a new component and create its copy.
    /// </summary>
    public static T DeepCopy<T>(T c)
    {
        GameLogger.Verify(c is not null);

        if (System.Text.Json.JsonSerializer.Deserialize<T>(
            System.Text.Json.JsonSerializer.Serialize(c, Game.Data.SerializationOptions), Game.Data.SerializationOptions) is not T obj)
        {
            throw new InvalidOperationException($"Unable to serialize {c.GetType().Name} for editor!?");
        }

        return obj;
    }

    internal static void HandleSerializationError<T>(object? _, T e)
    {
        if (e is not Newtonsoft.Json.Serialization.ErrorEventArgs error ||
            error.ErrorContext.Member is not string memberName ||
            error.CurrentObject is null)
        {
            // We can't really do much about it :(
            return;
        }

        Type targetType = error.CurrentObject.GetType();
        GameLogger.Error($"Error while loading field {memberName} of {targetType.Name}! Did you try setting PreviousSerializedName attribute?");

        // Let the editor handle this by propagating the exception.
        error.ErrorContext.Handled = Game.Data.IgnoreSerializationErrors;

        Game.Data.OnErrorLoadingAsset();
    }

    #endregion

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

    private static readonly Dictionary<Type, List<JsonPropertyInfo>?> _types = [];

    public static void AddPrivateFieldsModifier(JsonTypeInfo jsonTypeInfo)
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
                        fieldName = fieldName[1..];
                    }

                    if (existingProperties.Contains(fieldName))
                    {
                        continue;
                    }

                    JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(field.FieldType, fieldName);

                    jsonPropertyInfo.Get = field.GetValue;
                    jsonPropertyInfo.Set = field.SetValue;

                    jsonTypeInfo.Properties.Add(jsonPropertyInfo);

                    extraPrivateProperties ??= [];
                    extraPrivateProperties.Add(jsonPropertyInfo);

                    existingProperties.Add(jsonPropertyInfo.Name);
                }

                _types[t] = extraPrivateProperties;
            }

            t = t.BaseType;
        }
    }

    private static bool ShouldRemoveProperty(JsonPropertyInfo property, Type t)
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