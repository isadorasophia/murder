using System.ComponentModel;
using System.Reflection;

namespace Murder.Utilities
{
    public static class StringHelper
    {
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString()!);
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString()!;
        }

        public static string ToHumanList(this IEnumerable<string> someStringArray, string separator, string lastItemSeparator)
        {
            return string.Join($"{separator} ", someStringArray.Take(someStringArray.Count() - 1)) + (someStringArray.Count() <= 1 ? "" : $" {lastItemSeparator} ") + someStringArray.LastOrDefault();
        }

    }
}
