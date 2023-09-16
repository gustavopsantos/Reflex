using System;
using System.Collections.Generic;

namespace Reflex.Microsoft.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!source.TryGetValue(key, out TValue value))
            {
                value = valueFactory.Invoke(key);
                source.Add(key, value);
            }

            return value;
        }
    }
}