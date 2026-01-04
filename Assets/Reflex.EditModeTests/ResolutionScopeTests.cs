using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;

namespace Reflex.EditModeTests
{
    public class ResolutionScopeTests
    {
        public class ParentService
        {
        }

        public class ChildService
        {
        }

        public class ParentServiceThatDependsOnChildService
        {
            [Inject] public ChildService ChildService { get; private set; }
        }

        public class ChildServiceThatDependsOnParentService
        {
            [Inject] public ParentService ParentService { get; private set; }
        }

        public class ParentServiceThatDependsOnContainer
        {
            [Inject] public Container Container { get; private set; }
        }

        [Test]
        public void ChildServicesShouldHaveAccessToParentServices()
        {
            var parentContainer = new ContainerBuilder()
                .RegisterType(typeof(ParentService), Lifetime.Singleton, Resolution.Lazy)
                .Build();

            var childContainer = parentContainer.Scope(builder =>
                builder.RegisterType(typeof(ChildServiceThatDependsOnParentService), Lifetime.Singleton, Resolution.Lazy));

            childContainer.Resolve<ChildServiceThatDependsOnParentService>().ParentService.Should().NotBeNull();
        }

        [Test]
        public void ParentServiceShouldNotHaveAccessToChildContainer()
        {
            var parentContainer = new ContainerBuilder()
                .RegisterType(typeof(ParentServiceThatDependsOnContainer), Lifetime.Singleton, Resolution.Lazy)
                .Build();

            var childContainer = parentContainer.Scope();
            childContainer.Resolve<ParentServiceThatDependsOnContainer>().Container.Should().Be(parentContainer);
        }

        [Test]
        public void ParentServicesShouldNotHaveAccessToChildServices()
        {
            var parentContainer = new ContainerBuilder()
                .RegisterType(typeof(ParentServiceThatDependsOnChildService), Lifetime.Singleton, Resolution.Lazy)
                .Build();

            var childContainer = parentContainer.Scope(builder => builder.RegisterType(typeof(ChildService), Lifetime.Singleton, Resolution.Lazy));

            try
            {
                childContainer.Resolve<ParentServiceThatDependsOnChildService>();
                Assert.Fail("ParentService from parent container should not have access to ChildService from child container.");
            }
            catch (Exception e)
            {
                if (e is PropertyInjectorException propertyInjectorException &&
                    propertyInjectorException.InnerException is UnknownContractException unknownContractException &&
                    unknownContractException.UnknownContract == typeof(ChildService))
                {
                    Assert.Pass();
                }

                Assert.Fail(e.ToString());
            }
        }
    }
}