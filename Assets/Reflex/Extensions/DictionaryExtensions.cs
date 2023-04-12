using System;
using System.Collections.Generic;

namespace Reflex.Extensions
{
    internal static class DictionaryExtensions
    {
        public static void CopyAllFromExceptBy<TKey, TValue>(
            this IDictionary<TKey, TValue> dest,
            IDictionary<TKey, TValue> src,
            TKey except)
        {
            var comparer = EqualityComparer<TKey>.Default;

            foreach (var kvp in src)
            {
                if (!comparer.Equals(kvp.Key, except))
                {
                    dest[kvp.Key] = kvp.Value;
                }
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!source.TryGetValue(key, out var value))
            {
                value = valueFactory.Invoke(key);
                source.Add(key, value);
            }

            return value;
        }
    }
}