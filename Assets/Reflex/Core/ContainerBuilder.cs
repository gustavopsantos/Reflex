using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Enums;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Resolvers;
using UnityEngine.Assertions;

namespace Reflex.Core
{
    public sealed class ContainerBuilder
    {
        public string Name { get; private set; }
        public Container Parent { get; private set; }
        public List<Binding> Bindings { get; } = new();
        public event Action<Container> OnContainerBuilt;

        public Container Build()
        {
            var disposables = new DisposableCollection();
            var resolversByContract = new Dictionary<Type, List<IResolver>>();
            
            // Extra installers
            UnityInjector.ExtraInstallers?.Invoke(this);

            // Parent override
            if (UnityInjector.ContainerParentOverride.TryPeek(out var parentOverride))
            {
                Parent = parentOverride;
            }
            
            // Inherited resolvers
            if (Parent != null)
            {
                foreach (var (contract, resolvers) in Parent.ResolversByContract)
                {
                    resolversByContract[contract] = new List<IResolver>(resolvers);
                }
            }

            // Owned resolvers
            foreach (var binding in Bindings)
            {
                disposables.Add(binding.Resolver);

                foreach (var contract in binding.Contracts)
                {
                    if (!resolversByContract.TryGetValue(contract, out var resolvers))
                    {
                        resolvers = new List<IResolver>();
                        resolversByContract.Add(contract, resolvers);
                    }

                    resolvers.Add(binding.Resolver);
                }
            }
            
            var container = new Container(Name, Parent, resolversByContract, disposables);

            // Eagerly resolve inherited Scoped + Eager bindings
            if (Parent != null)
            {
                var inheritedEagerResolvers = Parent.ResolversByContract
                    .SelectMany(kvp => kvp.Value)
                    .ToHashSet()
                    .Where(r => r.Lifetime == Lifetime.Scoped && r.Resolution == Resolution.Eager);

                foreach (var resolver in inheritedEagerResolvers)
                {
                    resolver.Resolve(container);
                }
            }

            // Eagerly resolve self Singleton/Scoped + Eager bindings
            if (Bindings != null)
            {
                var selfEagerResolvers = Bindings
                    .Select(b => b.Resolver)
                    .Where(r => r.Resolution == Resolution.Eager && (r.Lifetime is Lifetime.Singleton or Lifetime.Scoped));

                foreach (var resolver in selfEagerResolvers)
                {
                    resolver.Resolve(container);
                }
            }

            Bindings.Clear();
            OnContainerBuilt?.Invoke(container);
            return container;
        }

        public ContainerBuilder SetName(string name)
        {
            Name = name;
            return this;
        }
        
        public ContainerBuilder SetParent(Container parent)
        {
            Parent = parent;
            return this;
        }
        
        public bool HasBinding(Type contract)
        {
            return Bindings.Any(binding => binding.Contracts.Contains(contract));
        }

        public ContainerBuilder RegisterType(Type type, Lifetime lifetime, Resolution resolution)
        {
            return RegisterType(type, new[] { type }, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type, Type[] contracts, Lifetime lifetime, Resolution resolution)
        {
            Assert.IsNotNull(type);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Type registration Lifetime.Transient + Resolution.Eager not allowed");

            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton => new SingletonTypeResolver(type, resolution),
                Lifetime.Transient => new TransientTypeResolver(type),
                Lifetime.Scoped => new ScopedTypeResolver(type, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterType() method.")
            };
            
            return Add(type, contracts, resolver);
        }
        
        public ContainerBuilder RegisterValue(object value, Lifetime lifetime)
        {
            return RegisterValue(value, new[] { value.GetType() }, lifetime);
        }
        
        public ContainerBuilder RegisterValue(object value, Type[] contracts, Lifetime lifetime)
        {
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsTrue(lifetime == Lifetime.Singleton, "Value registration only supports Lifetime.Singleton");
            var resolver = new SingletonValueResolver(value);
            return Add(value.GetType(), contracts, resolver);
        }

        public ContainerBuilder RegisterFactory<T>(Func<Container, T> factory, Lifetime lifetime, Resolution resolution)
        {
            return RegisterFactory(factory, new[] { typeof(T) }, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<T>(Func<Container, T> factory, Type[] contracts, Lifetime lifetime, Resolution resolution)
        {
            Assert.IsNotNull(factory);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Factory registration Lifetime.Transient + Resolution.Eager not allowed");
            
            object TypelessFactory(Container container)
            {
                return factory.Invoke(container);
            }
            
            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton =>  new SingletonFactoryResolver(TypelessFactory, resolution),
                Lifetime.Transient => new TransientFactoryResolver(TypelessFactory),
                Lifetime.Scoped => new ScopedFactoryResolver(TypelessFactory, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterFactory() method.")
            };

            return Add(typeof(T), contracts, resolver);
        }
        
        private ContainerBuilder Add(Type concrete, Type[] contracts, IResolver resolver)
        {
            var binding = Binding.Validated(resolver, concrete, contracts);
            Bindings.Add(binding);
            return this;
        }
    }
}