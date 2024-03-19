using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Extensions;

namespace Reflex.EditModeTests
{
    public class EnumerableExtensionsTests
    {
        private static readonly IEnumerable<object> _numbers = new List<object> {1, 2, 3, 42};

        [Test]
        public void AfterDynamicallyCasted_ShouldBeAssignable()
        {
            _numbers.CastDynamic(typeof(int)).GetType().Should().BeAssignableTo<IEnumerable<int>>();
        }

        [Test]
        public void AfterDynamicallyCasted_ShouldBeConvertible()
        {
            string.Join(",", (IEnumerable<int>) _numbers.CastDynamic(typeof(int))).Should().Be("1,2,3,42");
        }
    }
}