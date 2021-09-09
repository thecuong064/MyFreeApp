using System;

namespace MyNotes.Extensions
{
    public static class StringExtentions
    {
        public static int WordCount(this string str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string GetFirstSubstringWithEllipsis(this string str, int charCount = 0)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            if (charCount <= 0)
                return string.Empty;

            if (str.Length <= charCount)
                return str;

            return $"{str.Substring(0, charCount)}...";
        }
    }
}
