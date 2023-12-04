using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.Tests
{
    public class TransientValueResolverTests
    {
        [Test]
        public void AddTransientFromValue_FirstResolve_ShouldReturnValue()
        {
            var container = new ContainerDescriptor("")
                .AddTransient(42)
                .Build();

            container.Resolve<int>().Should().Be(42);
        }

        [Test]
        public void AddValueType_AsTransientFromValue_SecondResolve_ShouldThrow()
        {
            var container = new ContainerDescriptor("")
                .AddTransient(42)
                .Build();

            Action resolve = () => container.Resolve<int>();
            
            resolve.Should().NotThrow();
            resolve.Should().Throw<Exception>();
        }
        
        [Test]
        public void AddReferenceType_AsTransientFromValue_SecondResolve_ShouldThrow()
        {
            var container = new ContainerDescriptor("")
                .AddTransient(string.Empty)
                .Build();

            Action resolve = () => container.Resolve<string>();
            
            resolve.Should().NotThrow();
            resolve.Should().Throw<Exception>();
        }
    }
}