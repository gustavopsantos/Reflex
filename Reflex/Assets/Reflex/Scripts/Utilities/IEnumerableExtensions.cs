using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflex.Scripts.Utilities
{
    public static class IEnumerableExtensions
    {
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.OrderByDescending(selector).First();
        }
    }
}