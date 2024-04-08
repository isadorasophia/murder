using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Murder.Serialization;

public sealed class ComplexDictionaryConverter<T, V> : JsonConverter<ComplexDictionary<T, V>> where T : notnull
{
    private bool _initialized = false;

    private JsonConverter<T>? _keyConverter = null;
    private JsonConverter<V>? _valueResolver = null;

    public override ComplexDictionary<T, V>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        InitializeArrayResolver(options);

        if (_keyConverter is not JsonConverter<T> converterKey)
        {
            throw new InvalidOperationException($"Unable to serialize {GetType().Name}. Could not find a valid JsonConverter.");
        }

        if (_valueResolver is not JsonConverter<V> converterValue)
        {
            throw new InvalidOperationException($"Unable to serialize {GetType().Name}. Could not find a valid JsonConverter.");
        }

        ComplexDictionary<T, V> result = [];
        while (reader.Read()) // {
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            reader.Read(); // "key:"
            T? key = converterKey.Read(ref reader, typeof(T), options);

            reader.Read(); // "}"
            reader.Read(); // "value:"
            V? value = converterValue.Read(ref reader, typeof(V), options);

            if (key is null || value is null)
            {
                throw new JsonException($"Unable to read key or value for {typeToConvert.Name}");
            }

            result[key] = value;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ComplexDictionary<T, V> dictionary, JsonSerializerOptions options)
    {
        InitializeArrayResolver(options);

        if (_keyConverter is not JsonConverter<T> converterKey)
        {
            throw new InvalidOperationException($"Unable to serialize {GetType().Name}. Could not find a valid JsonConverter.");
        }

        if (_valueResolver is not JsonConverter<V> converterValue)
        {
            throw new InvalidOperationException($"Unable to serialize {GetType().Name}. Could not find a valid JsonConverter.");
        }

        writer.WriteStartObject();

        foreach ((T key, V value) in dictionary)
        {
            writer.WritePropertyName("key");
            converterKey.Write(writer, key, options);

            writer.WritePropertyName("value");
            converterValue.Write(writer, value, options);
        }

        writer.WriteEndObject();
    }

    private void InitializeArrayResolver(JsonSerializerOptions options)
    {
        if (!_initialized)
        {
            options.TryGetTypeInfo(typeof(T), out JsonTypeInfo? keyInfo);
            options.TryGetTypeInfo(typeof(V), out JsonTypeInfo? valueInfo);

            _keyConverter = keyInfo?.Converter as JsonConverter<T>;
            _valueResolver = valueInfo?.Converter as JsonConverter<V>;

            _initialized = true;
        }
    }
}