using System.Text.RegularExpressions;

namespace Murder.Utilities
{
    public static class CollectionHelper
    {
        /// <summary>
        /// Convert a collection to a dictionary with string keys that may have duplicate values.
        /// </summary>
        public static Dictionary<string, V> ToStringDictionary<T, V>(
            IEnumerable<T> collection,
            Func<T, string> toKey,
            Func<T, V> toValue)
        {
            Dictionary<string, V> result = new();

            ToStringDictionary(ref result, collection, toKey, toValue);
            return result;
        }

        public static Dictionary<string, V> ToStringDictionary<T, V>(
            ref Dictionary<string, V> existingDictionary,
            IEnumerable<T> collection,
            Func<T, string> toKey,
            Func<T, V> toValue)
        {
            foreach (T t in collection)
            {
                string key = GetNextValidName(existingDictionary, toKey(t));
                existingDictionary[key] = toValue(t);
            }

            return existingDictionary;
        }

        /// <summary>
        /// Get the next valid name on a collection of string keys.
        /// </summary>
        private static string GetNextValidName<T>(Dictionary<string, T> dictionary, string key, int depth = 0)
        {
            if (dictionary.ContainsKey(key))
            {
                if (Regex.Match(key, "([0-9]+)").Success)
                {
                    key = Regex.Replace(key, "([0-9]+)", $"{depth + 1}");
                }
                else
                {
                    key = key + " (1)";
                }

                key = GetNextValidName(dictionary, key, depth + 1);
            }

            return key;
        }
    }
}
