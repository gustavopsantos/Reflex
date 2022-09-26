using System.Collections.Generic;

namespace Reflex.Scripts.Utilities
{
    internal static class EnumerableUtilities
    {
        internal static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}