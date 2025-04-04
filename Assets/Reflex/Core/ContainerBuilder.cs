using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Enums;
using Reflex.Generics;
using Reflex.Resolvers;
using UnityEngine.Assertions;

namespace Reflex.Core
{
    public sealed class ContainerBuilder
    {
        public string Name { get; private set; }
        public Container Parent { get; private set; }
        public List<IBinding> Bindings { get; } = new();
        public event Action<Container> OnContainerBuilt;

        public Container Build()
        {
            var disposables = new DisposableCollection();
            var resolversByContract = new Dictionary<Type, List<IResolver>>();

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
        
        public bool HasBinding(Type type)
        {
            return Bindings.Any(binding => binding.Contracts.Contains(type));
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

            IBinding binding = lifetime switch
            {
                Lifetime.Singleton => Singleton.FromType(type, contracts, resolution),
                Lifetime.Transient => Transient.FromType(type, contracts),
                Lifetime.Scoped => Scoped.FromType(type, contracts, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterType() method.")
            };

            Bindings.Add(binding);
            return this;
        }
        
        public ContainerBuilder RegisterValue(object value, Lifetime lifetime)
        {
            return RegisterValue(value, new[] { value.GetType() }, lifetime);
        }
        
        public ContainerBuilder RegisterValue(object value, Type[] contracts, Lifetime lifetime)
        {
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsTrue(lifetime == Lifetime.Singleton, "Value registration only supports Lifetime.Singleton");
            
            IBinding binding = Singleton.FromValue(value, contracts);
            Bindings.Add(binding);
            return this;
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
            
            IBinding binding = lifetime switch
            {
                Lifetime.Singleton => Singleton.FromFactory(factory, contracts, resolution),
                Lifetime.Transient => Transient.FromFactory(factory, contracts),
                Lifetime.Scoped => Scoped.FromFactory(factory, contracts, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterFactory() method.")
            };
            
            Bindings.Add(binding);
            return this;
        }
    }
}