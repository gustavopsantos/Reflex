using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    public class ScopedEagerInitializationTests
    {
        class ScopedService
        {
            public static List<ScopedService> Instances { get; } = new(); 
            
            public ScopedService()
            {
                Instances.Add(this);
            }
        }

        [Test]
        public void EnsureScopedEagerInstancesAreConstructedAsSoonAsContainerIsBuilt()
        {
            ScopedService.Instances.Clear();
            
            var builder = new ContainerBuilder().RegisterType(typeof(ScopedService), Lifetime.Scoped, Resolution.Eager);
            ScopedService.Instances.Count.Should().Be(0);
            
            var containerA = builder.Build();
            ScopedService.Instances.Count.Should().Be(1);
            
            var scoped0 = containerA.Scope();
            var scoped1 = containerA.Scope();
            var scoped2 = scoped1.Scope();
            var scoped3 = scoped2.Scope();
            ScopedService.Instances.Count.Should().Be(5);
        }
    }
}