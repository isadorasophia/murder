using System.Collections.Immutable;

namespace Murder.Utilities
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Flag a switch with a chance of <paramref name="chance"/>%.
        /// </summary>
        /// <param name="random">The amound of odds of hitting that particular switch.</param>
        public static bool TryWithChanceOf(this Random random, int chance)
        {
            return random.Next(0, 100) <= chance;
        }

        public static T AnyOf<T>(this Random r, IList<T> arr)
        {
            return arr[r.Next(arr.Count)];
        }

        public static T AnyEnumOf<T>(this Random r) where T : struct, IConvertible
        {
            var allValues = Enum.GetValues(typeof(T));
            if (allValues.Length == 0)
            {
                throw new InvalidOperationException("Invalid call for getting a random enum!");
            }

            return (T)allValues.GetValue(r.Next(allValues.Length))!;
        }

        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }

        public static T PopRandom<T>(this List<T> list, Random random)
        {
            int i = random.Next(list.Count);
            var result = list[i];
            list.RemoveAt(i);

            return result;
        }

        public static U GetRandom<T, U>(this Dictionary<T, U> dict, Random random) where T : notnull
        {
            int i = random.Next(dict.Count);
            var result = dict.ElementAt(i);

            return dict[result.Key];
        }

        public static U PopRandom<T, U>(this Dictionary<T, U> dict, Random random) where T : notnull
        {
            int i = random.Next(dict.Count);
            var result = dict.ElementAt(i);
            dict.Remove(result.Key);

            return result.Value;
        }

        public static T GetRandom<T>(this ImmutableArray<T> array, Random random)
        {
            int i = random.Next(array.Length);
            return array[i];
        }
    }
}
