using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Reflex.Core;
using NUnit.Framework;

namespace Reflex.Tests
{
    internal class EnumerableInjectionTests
    {
        private class NumberManager
        {
            public IEnumerable<int> Numbers { get; }

            public NumberManager(IEnumerable<int> numbers)
            {
                Numbers = numbers;
            }
        }
        
        [Test]
        public void Container_ShouldBeAbleToConstructObjectWithIEnumerableDependency()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(1)
                .AddSingleton(2)
                .AddSingleton(3)
                .Build();
            
            Action construct = () => container.Construct<NumberManager>();
            construct.Should().NotThrow();
        }

        [Test]
        public void NestedEnumerableShouldBeSupported()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(new List<int> {1, 2, 3})
                .AddSingleton(new List<int> {4, 5, 6})
                .Build();

            container.All<List<int>>().SelectMany(x => x).Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 6});
        }
    }
}