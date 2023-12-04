using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.Tests
{
    public class DecorateTests
    {
        public interface INumber
        {
            int Get();
        }

        public class Number : INumber
        {
            private int _value;
            public int Get() => _value;

            public static Number FromValue(int value)
            {
                return new Number
                {
                    _value = value
                };
            }
        }
        
        public class DoubledNumber : INumber
        {
            private readonly INumber _number;
            public DoubledNumber(INumber number) => _number = number;
            public int Get() => _number.Get() * 2;
        }
        
        public class HalvedNumber : INumber
        {
            private readonly INumber _number;
            public HalvedNumber(INumber number) => _number = number;
            public int Get() => _number.Get() / 2;
        }

        public interface IManager
        {
        }

        public interface IBundleManager
        {
        }

        public class BundleManager : IBundleManager, IManager
        {
        }

        public class ResilientBundleManager : IBundleManager, IManager
        {
            public ResilientBundleManager(IBundleManager bundleManager)
            {
            }
        }
        
        [Test]
        public void Decorate_ShouldDecorateAllMatchingContracts()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(Number.FromValue(1), contracts: typeof(INumber))
                .AddSingleton(Number.FromValue(2), contracts: typeof(INumber))
                .AddSingleton(Number.FromValue(3), contracts: typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .Build();

            var numbers = container.All<INumber>().Select(n => n.Get()).ToArray();
            numbers.Length.Should().Be(3);
            numbers.Should().BeEquivalentTo(new int[] {2, 4, 6});
        }
        
        [Test]
        public void Decorate_ShouldBeAbleToNestDecorations()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(Number.FromValue(10), contracts: typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .AddDecorator(typeof(HalvedNumber), typeof(INumber))
                .AddDecorator(typeof(HalvedNumber), typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .Build();

            var number = container.Single<INumber>();
            number.Get().Should().Be(80);
        }

        [Test]
        public void DecoratedContract_ShouldReplaceOnlyDecoratedContract()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(typeof(BundleManager), typeof(IBundleManager), typeof(IManager))
                .AddDecorator(typeof(ResilientBundleManager), typeof(IBundleManager))
                .Build();

            container.Resolve<IManager>().GetType().Should().Be<BundleManager>();
            container.Resolve<IBundleManager>().GetType().Should().Be<ResilientBundleManager>();
        }
        
        [Test]
        public void DecoratedSingleton_ShouldDecorateWithSingleton()
        {
            var container = new ContainerDescriptor("")
                .AddSingleton(typeof(Number), contracts: typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .Build();

            var distinctInstances = new HashSet<INumber>
            {
                container.Resolve<INumber>(),
                container.Resolve<INumber>(),
                container.Resolve<INumber>(),
            };

            distinctInstances.Count.Should().Be(1);
        }
        
        [Test]
        public void DecoratedTransient_ShouldDecorateWithTransient()
        {
            var container = new ContainerDescriptor("")
                .AddTransient(typeof(Number), contracts: typeof(INumber))
                .AddDecorator(typeof(DoubledNumber), typeof(INumber))
                .Build();

            var distinctInstances = new HashSet<INumber>
            {
                container.Resolve<INumber>(),
                container.Resolve<INumber>(),
                container.Resolve<INumber>(),
            };

            distinctInstances.Count.Should().Be(3);
        }
    }
}