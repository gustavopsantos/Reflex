using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;

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
                .AddSingleton(typeof(ParentService))
                .Build();

            var childContainer = parentContainer.Scope(builder => builder.AddSingleton(typeof(ChildServiceThatDependsOnParentService)));
            
            childContainer.Resolve<ChildServiceThatDependsOnParentService>().ParentService.Should().NotBeNull();
        }

        [Test]
        public void ParentServicesShouldNotHaveAccessToChildServices()
        {
            var parentContainer = new ContainerBuilder()
                .AddSingleton(typeof(ParentServiceThatDependsOnChildService))
                .Build();

            var childContainer = parentContainer.Scope(builder => builder.AddSingleton(typeof(ChildService)));
            childContainer.Resolve<ParentServiceThatDependsOnChildService>().ChildService.Should().BeNull();
        }

        [Test]
        public void ParentServiceShouldNotHaveAccessToChildContainer()
        {
            var parentContainer = new ContainerBuilder()
                .AddSingleton(typeof(ParentServiceThatDependsOnContainer))
                .Build();

            var childContainer = parentContainer.Scope();
            childContainer.Resolve<ParentServiceThatDependsOnContainer>().Container.Should().Be(parentContainer);
        }
    }
}