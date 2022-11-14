using System.Text;
using System.Text.RegularExpressions;

namespace Murder.Editor.Utilities
{
    internal static class Prettify
    {
        public static string CapitalizeFirstLetter(string name)
        {
            // Remove underscores.
            name = Extract(name, new(@"(?<=_)(.*)"));

            // Remove "Guid" from the name.
            name = Extract(name, new(@"(.*)(?=Guid)"));
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

            return name;
        }
        public static string FormatName(string name)
        {
            // Remove underscores.
            name = Extract(name, new(@"(?<=_)(.*)"));

            // Remove "Guid" from the name.
            name = Extract(name, new(@"(.*)(?=Guid)"));

            // Replace uppercase for spaces.
            name = DoSpacesForUppercase(name);

            return name;
        }

        /// <summary>
        /// Extract the group in the provided regex.
        /// If it matches, this returns the formatted <paramref name="name"/>.
        /// Otherwise, return <paramref name="name"/>.
        /// </summary>
        private static string Extract(string name, Regex re)
        {
            Match m = re.Match(name);
            if (m.Success)
            {
                string result = m.Groups[0].Value;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return m.Groups[0].Value;
                }
            }

            return name;
        }

        private static string DoSpacesForUppercase(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            StringBuilder result = new();

            // Always upper case first letter.
            result.Append(char.ToUpper(text[0]));

            for (int i = 1; i < text.Length; ++i)
            {
                if (char.IsUpper(text[i]) && !char.IsUpper(text[i-1]))
                {
                    result.Append(' ');
                }

                result.Append(text[i]);
            }

            return result.ToString();
        }
    }
}
