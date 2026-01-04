using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    internal class MultipleContractTests
    {
        private interface IManager
        {
        }

        private interface IBundleManager : IManager
        {
        }

        private interface IPrefabManager : IManager
        {
        }

        private class BundleManager : IBundleManager
        {
        }

        private class PrefabManager : IPrefabManager
        {
        }

        [Test]
        public void SingletonWithMultipleContractsCanBeResolved()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(BundleManager), new[] { typeof(IBundleManager), typeof(IManager) }, Lifetime.Singleton, Resolution.Lazy)
                .RegisterType(typeof(PrefabManager), new[] { typeof(IPrefabManager), typeof(IManager) }, Lifetime.Singleton, Resolution.Lazy)
                .Build();

            var bundleManager = container.Single<IBundleManager>();
            var prefabManager = container.Single<IPrefabManager>();

            var managers = container.All<IManager>().ToArray();
            managers.Length.Should().Be(2);
            managers.Should().Contain(new IManager[] {bundleManager, prefabManager});
        }
    }
}