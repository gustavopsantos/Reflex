using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class OnInjectedListenerTests
    {
        private static class StaticEventBus
        {
            public static event Action<char> OnInjectedEvent;
            public static void RaiseOnInjectedEvent(char id) => OnInjectedEvent?.Invoke(id);
        }

        private abstract class AttributeInjected : IOnInjectedListener
        {
            private readonly char _id;
            protected AttributeInjected(char id) => _id = id;
            public void OnInjected() => StaticEventBus.RaiseOnInjectedEvent(_id);
        }
        
        private class AttributeInjectedA : AttributeInjected
        {
            [Inject] private readonly AttributeInjectedB _b;
            public AttributeInjectedA() : base('a') { }
        }
        
        private class AttributeInjectedB : AttributeInjected
        {
            [Inject] private readonly AttributeInjectedC _c;
            public AttributeInjectedB() : base('b') { }
        }
        
        private class AttributeInjectedC : AttributeInjected
        {
            public AttributeInjectedC() : base('c') { }
        }
        
        private class ConstructorInjectedA : AttributeInjected
        {
            private readonly ConstructorInjectedB _b;
            public ConstructorInjectedA(ConstructorInjectedB b) : base('a') => _b = b;
        }
        
        private class ConstructorInjectedB : AttributeInjected
        {
            private readonly ConstructorInjectedC _c;
            public ConstructorInjectedB(ConstructorInjectedC c) : base('b') => _c = c;
        }
        
        private class ConstructorInjectedC : AttributeInjected
        {
            public ConstructorInjectedC() : base('c') { }
        }

        [Test]
        public void AttributeInjectedObjects_GetsOnInjectedRaisedRecursively()
        {
            var constructionOrder = new List<char>(3);
            StaticEventBus.OnInjectedEvent -= UpdateOnInjectedOrder;
            StaticEventBus.OnInjectedEvent += UpdateOnInjectedOrder;
            void UpdateOnInjectedOrder(char service) => constructionOrder.Add(service);

            var container = new ContainerBuilder()
                .RegisterType(typeof(AttributeInjectedA), Lifetime.Singleton, Resolution.Lazy)
                .RegisterType(typeof(AttributeInjectedB), Lifetime.Singleton, Resolution.Lazy)
                .RegisterType(typeof(AttributeInjectedC), Lifetime.Singleton, Resolution.Lazy)
                .Build();
            
            container.Resolve<AttributeInjectedA>();
            string.Join(string.Empty, constructionOrder).Should().BeEquivalentTo("cba");
        }
        
        [Test]
        public void ConstructorInjectedObjects_GetsOnInjectedRaisedRecursively()
        {
            var constructionOrder = new List<char>(3);
            StaticEventBus.OnInjectedEvent -= UpdateOnInjectedOrder;
            StaticEventBus.OnInjectedEvent += UpdateOnInjectedOrder;
            void UpdateOnInjectedOrder(char service) => constructionOrder.Add(service);

            var container = new ContainerBuilder()
                .RegisterType(typeof(ConstructorInjectedA), Lifetime.Singleton, Resolution.Lazy)
                .RegisterType(typeof(ConstructorInjectedB), Lifetime.Singleton, Resolution.Lazy)
                .RegisterType(typeof(ConstructorInjectedC), Lifetime.Singleton, Resolution.Lazy)
                .Build();
            
            container.Resolve<ConstructorInjectedA>();
            string.Join(string.Empty, constructionOrder).Should().BeEquivalentTo("cba");
        }
    }
}