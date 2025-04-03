using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    public class RecursiveConstructionTreeTests
    {
        public class ServiceOne
        {
            private readonly float _a;
            private readonly ServiceTwo _b;

            public ServiceOne(float a, ServiceTwo b)
            {
                _a = a;
                _b = b;
            }
        }

        public class ServiceTwo
        {
            private readonly int _a;
            private readonly int _b;

            public ServiceTwo(int a, int b)
            {
                _a = a;
                _b = b;
            }
        }

        [Test]
        public void Resolve_RecursiveConstructionTree_ShouldNotThrow()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(42))
                .Add(Singleton.FromValue(1.5f))
                .Add(Singleton.FromType(typeof(ServiceOne), Resolution.Lazy))
                .Add(Singleton.FromType(typeof(ServiceTwo), Resolution.Lazy))
                .Build();

            Action resolve = () => container.Resolve<ServiceOne>();
            resolve.Should().NotThrow();
        }
    }
}