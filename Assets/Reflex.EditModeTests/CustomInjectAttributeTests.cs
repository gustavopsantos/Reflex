using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class CustomInjectAttributeTests
    {
        private class CustomInjectAttribute : InjectAttribute
        {
        }

        public class CustomInjectOnField
        {
            [CustomInject] private int _number;
            public int GetNumber() => _number;
        }

        public class CustomInjectOnProperty
        {
            [CustomInject] private int Number { get; set; }
            public int GetNumber() => Number;
        }

        public class CustomInjectOnMethod
        {
            private int _number;

            [CustomInject]
            private void Inject(int number)
            {
                _number = number;
            }

            public int GetNumber() => _number;
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectFields()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42, Lifetime.Singleton)
                .Build();

            var service = container.Construct<CustomInjectOnField>();
            service.GetNumber().Should().Be(42);
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectProperties()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42, Lifetime.Singleton)
                .Build();

            var service = container.Construct<CustomInjectOnProperty>();
            service.GetNumber().Should().Be(42);
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectMethods()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42, Lifetime.Singleton)
                .Build();

            var service = container.Construct<CustomInjectOnMethod>();
            service.GetNumber().Should().Be(42);
        }
    }
}