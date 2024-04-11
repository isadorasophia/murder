using Bang;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Serialization;
using Newtonsoft.Json;
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

        var settings = FileHelper._settings;
        if (JsonConvert.DeserializeObject(JsonConvert.SerializeObject(c, settings), c.GetType(), settings) is not T obj)
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

    private static readonly Assembly _systemAssembly = typeof(object).Assembly;
    private static readonly Dictionary<Type, List<JsonPropertyInfo>?> _typeProperties = new();

    public static void AddPrivateFieldsModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        Type? t = jsonTypeInfo.Type;

        while (t is not null && t.Assembly != _systemAssembly)
        {
            if (_typeProperties.TryGetValue(t, out List<JsonPropertyInfo>? properties))
            {
                //if (properties is not null)
                //{
                //    foreach (JsonPropertyInfo property in properties)
                //    {
                //        jsonTypeInfo.Properties.Add(property);
                //    }
                //}

                // This means that the type cache fields were already added? I guess?
                break;
            }
            else
            {
                bool fetchedConstructors = false;
                HashSet<string>? parameters = null;

                // Slightly evil in progress code. If the field is *not* found as any of the constructor parameters,
                // manually use the seter via reflection. I don't care.
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

                HashSet<string>? existingFields = null;
                List<JsonPropertyInfo>? extraFields = null;

                // Now, this is okay. There is not much to do here. If the field is private, manually fallback to reflection.
                foreach (FieldInfo field in t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (!Attribute.IsDefined(field, typeof(SerializeAttribute)) && !Attribute.IsDefined(field, typeof(ShowInEditorAttribute)))
                    {
                        continue;
                    }

                    string fieldName = field.Name;

                    existingFields ??= FetchPropertiesForJsonCache(jsonTypeInfo.Properties);
                    if (existingFields.Contains(fieldName))
                    {
                        continue;
                    }

                    // We may need to manually format names for private fields.
                    if (fieldName.StartsWith('_'))
                    {
                        fieldName = fieldName[1..];

                        // If there is another property with the same type, we certainly don't want to create a duplicate name.
                        // This will make System.Text.Json throw an exception, so we have to manually workaround this.
                        if (existingFields.Contains(fieldName))
                        {
                            fieldName = field.Name;
                        }
                    }

                    existingFields.Add(fieldName);

                    JsonPropertyInfo jsonPropertyInfo = jsonTypeInfo.CreateJsonPropertyInfo(field.FieldType, fieldName);

                    jsonPropertyInfo.Get = field.GetValue;
                    jsonPropertyInfo.Set = field.SetValue;

                    jsonTypeInfo.Properties.Add(jsonPropertyInfo);

                    //extraFields ??= [];
                    //extraFields.Add(jsonPropertyInfo);
                }

                _typeProperties.Add(t, extraFields);
            }

            t = t.BaseType;
        }
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

    private static HashSet<string> FetchPropertiesForJsonCache(IList<JsonPropertyInfo> properties)
    {
        HashSet<string> set = new(properties.Count, StringComparer.OrdinalIgnoreCase);
        foreach (JsonPropertyInfo property in properties)
        {
            _ = set.Add(property.Name);
        }

        return set;
    }
}