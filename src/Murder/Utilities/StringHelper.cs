using Murder.Core.Graphics;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Murder.Utilities
{
    public static partial class StringHelper
    {
        public static bool FuzzyMatch(string searchTerm, string target)
        {
            int searchTermIndex = 0;

            foreach (char targetChar in target)
            {
                if (searchTermIndex < searchTerm.Length && char.ToLowerInvariant(targetChar) == char.ToLowerInvariant(searchTerm[searchTermIndex]))
                {
                    searchTermIndex++;
                }
            }

            return searchTermIndex == searchTerm.Length;
        }

        public static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int[] v0 = new int[t.Length + 1];
            int[] v1 = new int[t.Length + 1];

            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                v1[0] = i + 1;

                for (int j = 0; j < t.Length; j++)
                {
                    int cost = (s[i] == t[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
                }

                int[] temp = v0;
                v0 = v1;
                v1 = temp;
            }

            return v0[t.Length];
        }

        public static TAttr? GetAttribute<T, TAttr>(this T enumerationValue) where T : Enum
        {
            Type type = enumerationValue.GetType();

            // Tries to find a DescriptionAttribute for a potential friendly name
            // for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                if (memberInfo[0].GetCustomAttribute(typeof(TAttr), false) is TAttr attr)
                {
                    // Pull out the description value
                    return attr;
                }
            }

            // If we have no description attribute, just return the ToString of the enum
            return default;
        }

        public static string GetDescription<T>(this T enumerationValue) where T : Enum
        {
            Type type = enumerationValue.GetType();

            // Tries to find a DescriptionAttribute for a potential friendly name
            // for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                if (memberInfo[0].GetCustomAttribute(typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
                {
                    // Pull out the description value
                    return attr.Description;
                }
            }

            // If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        /// <summary>
        /// Removes single returns, keeps doubles.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Cleanup(this string input)
        {
            // Replace the found characters with an empty string
            string output = CleanupReturnsCharacters().Replace(input, " ");

            // Replace the found characters with a single '\n' character
            // output = Regex.Replace(output, "\n(?!\n)|\n\n", "\n");

            return output;
        }

        public static string ToHumanList(this IEnumerable<string> someStringArray, string separator, string lastItemSeparator)
        {
            return string.Join($"{separator} ", someStringArray.Take(someStringArray.Count() - 1)) + (someStringArray.Count() <= 1 ? "" : $" {lastItemSeparator} ") + someStringArray.LastOrDefault();
        }

        [Flags]
        public enum StringHelperCapitalizeFlags
        {
            FirstOnly = 1,
            AfterSpace = 0b10,
            TrimSpaces = 0b100
        }

        public static string CapitalizeFirstLetter(string input, StringHelperCapitalizeFlags flags = StringHelperCapitalizeFlags.FirstOnly)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (flags.HasFlag(StringHelperCapitalizeFlags.FirstOnly))
            {
                return CapitalizeWord(input);
            }

            if (flags.HasFlag(StringHelperCapitalizeFlags.TrimSpaces))
            {
                input = TextDataServices.TrimSpaces().Replace(input, " ");
                if (input.Length > 0 && input[0] == ' ')
                {
                    input = input[1..];
                }
            }

            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = CapitalizeWord(words[i]);
            }

            return string.Join(' ', words);
        }

        private static string CapitalizeWord(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char upperLetter = char.ToUpper(input[0]);
            if (input[0] == upperLetter)
            {
                // Already capitalized.
                return input;
            }

            if (input.Length == 1)
            {
                // Return single letter.
                return char.ToUpper(input[0]).ToString();
            }

            return $"{char.ToUpper(input[0])}{input[1..]}";
        }

        [GeneratedRegex("(?<!\n)\n(?!\n)")]
        private static partial Regex CleanupReturnsCharacters();
    }
}