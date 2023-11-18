// Ignore Spelling: MVBP

namespace MVBP.Extensions
{
    internal static class StringExtensions
    {
        internal static bool ContainsAny(this string str, params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (str.Contains(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool EndsWithAny(this string str, params string[] suffixes)
        {
            foreach (var substring in suffixes)
            {
                if (str.EndsWith(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool StartsWithAny(this string str, params string[] suffixes)
        {
            foreach (var substring in suffixes)
            {
                if (str.StartsWith(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static string RemoveSuffix(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }

        internal static string RemovePrefix(this string s, string prefix)
        {
            if (s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length, s.Length - prefix.Length);
            }
            return s;
        }

        internal static string CapitalizeFirstLetter(this string s)
        {
            if (s.Length == 0)
                return s;
            else if (s.Length == 1)
                return $"{char.ToUpper(s[0])}";
            else
                return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}