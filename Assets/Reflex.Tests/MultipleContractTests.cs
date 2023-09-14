//using System.Linq;
//using FluentAssertions;
//using Reflex.Core;
//using NUnit.Framework;

//namespace Reflex.Tests
//{
//    internal class MultipleContractTests
//    {
//        private interface IManager
//        {
//        }

//        private interface IBundleManager : IManager
//        {
//        }

//        private interface IPrefabManager : IManager
//        {
//        }

//        private class BundleManager : IBundleManager
//        {
//        }

//        private class PrefabManager : IPrefabManager
//        {
//        }

//        [Test]
//        public void SingletonWithMultipleContractsCanBeResolved()
//        {
//            var container = new ContainerDescriptor("")
//                .AddSingleton(typeof(BundleManager), typeof(IBundleManager), typeof(IManager))
//                .AddSingleton(typeof(PrefabManager), typeof(IPrefabManager), typeof(IManager))
//                .Build();

//            var bundleManager = container.Single<IBundleManager>();
//            var prefabManager = container.Single<IPrefabManager>();

//            var managers = container.All<IManager>().ToArray();
//            managers.Length.Should().Be(2);
//            managers.Should().Contain(new IManager[] {bundleManager, prefabManager});
//        }
//    }
//}