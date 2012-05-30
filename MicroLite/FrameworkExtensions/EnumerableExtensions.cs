namespace MicroLite.FrameworkExtensions
{
    using System;
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        internal static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}