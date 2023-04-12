using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflex.Extensions
{
    internal static class ListExtensions
    {
        internal static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
        
        internal static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.OrderByDescending(selector).First();
        }
    }
}