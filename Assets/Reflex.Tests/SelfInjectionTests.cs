using FluentAssertions;
using Reflex.Core;
using NUnit.Framework;

namespace Reflex.Tests
{
    internal class SelfInjectionTests
    {
        [Test]
        public void Container_ShouldBeAbleToResolveItself()
        {
            var container = new ContainerDescriptor("").Build();
            container.Single<Container>().Should().Be(container);
        }
    }
}