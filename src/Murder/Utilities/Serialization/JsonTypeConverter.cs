using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Murder.Serialization;

public class JsonTypeConverter : JsonConverter<Type>
{
    // TODO, caution: Deserialization of type instances like this is not recommended and should be avoided,
    // since it can lead to potential security issues.

    // If you really want this supported (for instance if the JSON input is trusted)

    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? assemblyQualifiedName = reader.GetString();
        if (assemblyQualifiedName is null || Type.GetType(assemblyQualifiedName) is not Type t)
        {
            throw new NotSupportedException("Invalid type for json.");
        }

        return t;
    }

    public override Type ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }

    public override void Write( 
        Utf8JsonWriter writer,
        Type value,
        JsonSerializerOptions options)
    {
        string? assemblyQualifiedName = value.AssemblyQualifiedName;
        writer.WriteStringValue(assemblyQualifiedName);
    }

    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        [DisallowNull] Type value,
        JsonSerializerOptions options)
    {
        string? assemblyQualifiedName = value.AssemblyQualifiedName;
        writer.WritePropertyName(assemblyQualifiedName ?? string.Empty);
    }
}
