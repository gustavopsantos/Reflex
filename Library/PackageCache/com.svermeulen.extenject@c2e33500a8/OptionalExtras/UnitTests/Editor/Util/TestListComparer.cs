using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests
{
    public static class TestListComparer
    {
        public static bool ContainSameElements(IEnumerable listA, IEnumerable listB)
        {
            return ContainSameElementsInternal(listA.Cast<object>().ToList(), listB.Cast<object>().ToList());
        }

        static bool ContainSameElementsInternal(
            List<object> listA, List<object> listB)
        {
            // We don't care how they are sorted as long as they are sorted the same way so just use hashcode
            Comparison<object> comparer = (object left, object right) => (left.GetHashCode().CompareTo(right.GetHashCode()));

            listA.Sort(comparer);
            listB.Sort(comparer);

            return Enumerable.SequenceEqual(listA, listB);
        }

        public static string PrintList<T>(List<T> list)
        {
            return list.Select(x => x.ToString()).ToArray().Join(",");
        }
    }
}
