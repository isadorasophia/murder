namespace Murder.Serialization
{
    /// <summary>
    /// When serializing dictionaries, System.Text.Json is not able to resolve custom dictionary keys.
    /// As a workaround for that, we will implement our own complex dictionary converter that manually deserializes
    /// each key and value.
    /// </summary>
    public class ComplexDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        public ComplexDictionary() : base() { }

        public ComplexDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public ComplexDictionary(IEqualityComparer<TKey>? comparer) : base(comparer) { }
    }
}