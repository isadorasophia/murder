using System.Text;
using System.Text.RegularExpressions;

namespace Murder.Editor.Utilities
{
    public static class Prettify
    {
        public static string FormatVariableName(string name)
        {
            // Remove underscores.
            name = Extract(name, new(@"(?<=_)(.*)"));

            // Remove "Guid" from the name.
            name = Extract(name, new(@"(.*)(?=Guid)"));

            // Add spaces between CamelCase
            name = Regex.Replace(name, "(?<=[a-z])([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled);

            // Capitalize first letter
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

            return name;
        }


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

            int maxLength = 9;
            int i = maxLength; //maximum length
            while (i < name.Length)
            {
                int nextSpace = name.IndexOf(' ', i);
                if (nextSpace == -1)
                    break;

                name = name.Remove(nextSpace, 1).Insert(nextSpace, "\n");
                i = nextSpace + maxLength + 1;
            }


            return name;
        }

        public static string FormatAssetName(string name)
        {
            // Pretty format and remove "Asset" from the name.
            name = Extract(FormatName(name), new(@"(.*)(?=Asset)"));

            // Lowercase all separate words.
            name = LowercaseWords(name, ' ');

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
                if (char.IsUpper(text[i]) && !char.IsUpper(text[i - 1]))
                {
                    result.Append(' ');
                }

                result.Append(text[i]);
            }

            return result.ToString();
        }

        private static string LowercaseWords(string text, char separator)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            StringBuilder result = new();

            string[] words = text.Split(separator);
            for (int i = 0; i < words.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(words[i]))
                {
                    continue;
                }

                if (i != 0)
                {
                    result.Append(' ');
                }

                result.Append(char.ToLower(words[i][0]));
                result.Append(words[i][1..]);
            }

            return result.ToString();
        }

        public static string FormatNameWithoutSuffix(string name, string suffix)
        {
            // Remove suffix from the name.
            name = Extract(name, new($"(.*)(?={suffix})"));

            return name;
        }
    }
}