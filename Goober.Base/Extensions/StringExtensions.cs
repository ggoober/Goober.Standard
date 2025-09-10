using System;
using System.Text.RegularExpressions;

namespace Goober.Base.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _rxDoubleSpaces = new Regex("[ ]{2,}", RegexOptions.Compiled);

        public static string RegexTemplate = @"[ ]{2,}";

        public static bool IsNullOrWhitespace(this string value)
        {
	        return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string TrimSafety(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Trim();
        }

        public static string ToLowerAndTrimSafety(this string value)
        {
            if (string.IsNullOrEmpty(value) == true)
                return string.Empty;

            return value.ToLower().Trim();
        }

        public static string RemoveDoubleSpacesSafety(this string value)
        {
            if (value == null)
                return null;

            return _rxDoubleSpaces.Replace(value, " ");
        }

        public static string SubstringSafety(this string str, int length)
        {
            return str?.Substring(0, Math.Min(length, str.Length));
        }

        /// <summary>
        /// Сравнение строк без учета регистра
        /// </summary>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str, string value)
        {
            return string.Equals(value, str, StringComparison.OrdinalIgnoreCase);
        }
    }
}
