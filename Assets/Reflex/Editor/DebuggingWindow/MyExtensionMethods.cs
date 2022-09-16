using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflex.Editor.DebuggingWindow
{
    public static class MyExtensionMethods
    {
        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            return ascending
                ? source.OrderBy(selector)
                : source.OrderByDescending(selector);
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            return ascending
                ? source.ThenBy(selector)
                : source.ThenByDescending(selector);
        }
    }
}