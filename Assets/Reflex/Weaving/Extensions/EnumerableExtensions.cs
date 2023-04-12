using System;
using System.Collections.Generic;

namespace Reflex.Weaving.Extensions
{
    internal static class EnumerableExtensions
    {
        public static string AsCSV<T>(this IEnumerable<T> source)
        {
            return string.Join(", ", source);
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}