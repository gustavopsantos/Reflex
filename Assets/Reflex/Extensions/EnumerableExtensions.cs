using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflex.Extensions
{
    internal static class EnumerableExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable, object>> _enumerableCastDelegates = new();
        private static readonly MethodInfo _enumerableCastMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))!;

        public static object CastDynamic(this IEnumerable source, Type target)
        {
            var castDelegate = _enumerableCastDelegates.GetOrAdd(target, t => _enumerableCastMethodInfo
                .MakeGenericMethod(t)
                .CreateDelegate<Func<IEnumerable, object>>());

            return castDelegate(source);
        }
        
        public static IEnumerable<T> Reversed<T>(this IList<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
    }
}