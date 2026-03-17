using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    public class FactoryInjectionTests
    {
        public class ServiceBuiltByUser
        {
            [Inject] public FooService FooService { get; private set; }
        }

        public class FooService
        {
        }

        [Test]
        [TestCase(Lifetime.Singleton)]
        [TestCase(Lifetime.Scoped)]
        [TestCase(Lifetime.Transient)]
        public void SingletonFactory_MustBeAutomaticallyInjected(Lifetime lifetime)
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(FooService), Lifetime.Singleton, Resolution.Lazy)
                .RegisterFactory(_ => new ServiceBuiltByUser(), lifetime, Resolution.Lazy)
                .Build();

            var serviceBuiltByUser = container.Resolve<ServiceBuiltByUser>();
            serviceBuiltByUser.FooService.Should().NotBeNull();
        }
    }
}