using Newtonsoft.Json;

namespace Murder.Serialization
{
    /// <summary>
    /// When serializing dictionaries, Newtonsoft is not able to resolve custom dictionary keys.
    /// As a workaround for that, we will implement our own complex dictionary which serializes its keys
    /// as a value tuple, with <see cref="JsonArrayAttribute"/>.
    /// </summary>
    [JsonArray]
    public class ComplexDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull { }
}