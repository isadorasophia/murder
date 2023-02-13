using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Murder.Utilities
{
    public static class StringHelper
    {
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
            string output = Regex.Replace(input, "(?<!\n)\n(?!\n)", " ");

            // Replace the found characters with a single '\n' character
            // output = Regex.Replace(output, "\n(?!\n)|\n\n", "\n");

            return output;
        }

        public static string ToHumanList(this IEnumerable<string> someStringArray, string separator, string lastItemSeparator)
        {
            return string.Join($"{separator} ", someStringArray.Take(someStringArray.Count() - 1)) + (someStringArray.Count() <= 1 ? "" : $" {lastItemSeparator} ") + someStringArray.LastOrDefault();
        }

    }
}
