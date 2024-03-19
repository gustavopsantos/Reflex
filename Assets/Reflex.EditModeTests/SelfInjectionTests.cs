using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    internal class SelfInjectionTests
    {
        [Test]
        public void Container_ShouldBeAbleToResolveItself()
        {
            var container = new ContainerBuilder().Build();
            container.Single<Container>().Should().Be(container);
        }
    }
}