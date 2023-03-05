using Murder.Utilities.Attributes;

namespace Murder.Utilities
{
    public static class EnumHelper
    {
        public static string ToCustomString<T>(this T value) where T : Enum
        {
            var descriptionAttribute = (CustomNameAttribute[])typeof(T)
                .GetField(value.ToString())!
                .GetCustomAttributes(typeof(CustomNameAttribute), false);

            return descriptionAttribute.Length > 0 ? descriptionAttribute[0].Name : value.ToString();
        }
    }
}
