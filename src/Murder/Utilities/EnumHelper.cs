using Murder.Utilities.Attributes;
using Newtonsoft.Json.Linq;

namespace Murder.Utilities
{
    public static class EnumHelper
    {
        public static IEnumerable<string> GetNames<T>() where T : Enum
        {
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                yield return ToCustomString<T>((T)item);
            }
        }

        public static string ToCustomString<T>(this T value) where T : Enum
        {
            var descriptionAttribute = (CustomNameAttribute[])typeof(T)
                .GetField(value.ToString())!
                .GetCustomAttributes(typeof(CustomNameAttribute), false);

            return descriptionAttribute.Length > 0 ? descriptionAttribute[0].Name : value.ToString();
        }
    }
}