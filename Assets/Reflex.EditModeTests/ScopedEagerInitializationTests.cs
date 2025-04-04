using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.EditModeTests
{
    public class ScopedEagerInitializationTests
    {
        class ScopedService
        {
            public ScopedService()
            {
                Debug.Log($"ScopedService {GetHashCode()} created.");
            }
        }
        
        [Test]
        public void Foo()
        {
            var containerBuilder = new ContainerBuilder()
                .RegisterType(typeof(ScopedService), Lifetime.Scoped, Resolution.Lazy);

            // containerBuilder.OnContainerBuilt += OnContainerBuilt;
            //
            // void OnContainerBuilt(Container c)
            // {
            //     containerBuilder.OnContainerBuilt -= OnContainerBuilt;
            //     c.Resolve<ScopedService>();
            //     c.OnContainerScoped += OnContainerBuilt;
            // }
            
            var container = containerBuilder.Build();
            var scoped1 = container.Scope();
            var scoped2 = container.Scope();
            var scoped3 = scoped2.Scope();
            var scoped4 = scoped3.Scope();
        }
    }
}