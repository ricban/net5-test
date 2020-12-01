using System;
using System.Linq;
using System.Text;

namespace Covid19.Client.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Merge(this string target, string source, string separator = " ")
        {
            if (source.IsNullOrWhiteSpace())
            {
                return target;
            }

            var t = target.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var s = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(separator, t.Union(s));
        }

        public static string? Truncate(this string str, int maxLength)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            return str?.Length > maxLength ? $"{str.Substring(0, maxLength)}..." : str;
        }

        public static string ToCamelCase(this string str)
        {
            if (str.IsNullOrWhiteSpace() || str.Length < 2)
            {
                return str;
            }

            var words = str.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder(words[0].Substring(0, 1).ToLower() + words[0][1..], str.Length);

            for (int i = 1; i < words.Length; i++)
            {
                result.Append(words[i].Substring(0, 1).ToUpper()).Append(words[i][1..]);
            }

            return result.ToString();
        }
    }
}