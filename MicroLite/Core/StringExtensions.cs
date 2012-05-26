namespace MicroLite.Core
{
    using System.Globalization;

    internal static class StringExtensions
    {
        internal static string FormatWith(this string value, params string[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, value, args);
        }
    }
}