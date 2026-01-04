using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class EagerResolutionTests
    {
        public class Service
        {
            public static event Action<Service> OnServiceConstructed;
            
            public Service()
            {
                OnServiceConstructed?.Invoke(this);
            }
        }
        
        [Test]
        public void EagerSingletonShouldBeConstructedOnContainerBuild()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;
      
            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
            
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Eager)
                .Build();
            
            Service.OnServiceConstructed -= OnServiceConstructed;
            eagerlyConstructedServices.Should().NotBeEmpty();
            eagerlyConstructedServices.Single().Should().NotBeNull();
            eagerlyConstructedServices.Single().Should().Be(container.Resolve<Service>());
        }
        
        [Test]
        public void EagerSingletonShouldBeConstructedOnlyOnce()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;
      
            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
            
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Eager)
                .Build();

            var scoped1 = container.Scope();
            var scoped2 = scoped1.Scope();
            
            Service.OnServiceConstructed -= OnServiceConstructed;
            eagerlyConstructedServices.Count.Should().Be(1);
            eagerlyConstructedServices.Single().Should().Be(scoped2.Resolve<Service>());
        }
        
        [Test]
        public void EagerScopedShouldBeConstructedOncePerContainer()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;
      
            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
            
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Scoped, Resolution.Eager)
                .Build();

            var scoped1 = container.Scope();
            var scoped2 = scoped1.Scope();
            
            Service.OnServiceConstructed -= OnServiceConstructed;
            eagerlyConstructedServices.Count.Should().Be(3);
            eagerlyConstructedServices[0].Should().Be(container.Resolve<Service>());
            eagerlyConstructedServices[1].Should().Be(scoped1.Resolve<Service>());
            eagerlyConstructedServices[2].Should().Be(scoped2.Resolve<Service>());
        }
    }
}