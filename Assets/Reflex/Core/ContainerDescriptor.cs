using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reflex.Enums;
using Reflex.Exceptions;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public class ContainerDescriptor
    {
        private string _name;
        private Container _parent;
        private List<ResolverDescriptor> _descriptors = new();
        public event Action<Container> OnContainerBuilt;

        public ContainerDescriptor(string name, Container parent = null)
        {
            _name = name;
            _parent = parent;
        }

        public Container Build()
        {
            Build(out var disposables, out var resolversByContract, out var toStart);
            var container = new Container(_name, resolversByContract, disposables);
            container.SetParent(_parent);

            // Clear references
            _name = null;
            _parent = null;
            _descriptors = null;

            // Call initializers
            foreach (var startable in toStart.Select(r => (IStartable) r.Resolve(container)))
            {
                startable.Start();
            }

            OnContainerBuilt?.Invoke(container);
            return container;
        }

        public ContainerDescriptor AddSingleton(Type concrete, params Type[] contracts)
        {
            return Add(concrete, contracts, new SingletonTypeResolver(concrete));
        }

        public ContainerDescriptor AddSingleton(Type concrete)
        {
            return AddSingleton(concrete, concrete);
        }

        public ContainerDescriptor AddSingleton(object instance, params Type[] contracts)
        {
            return Add(instance.GetType(), contracts, new SingletonValueResolver(instance));
        }

        public ContainerDescriptor AddSingleton(object instance)
        {
            return AddSingleton(instance, instance.GetType());
        }

        public ContainerDescriptor AddSingleton<T>(Func<Container, T> factory, params Type[] contracts)
        {
            var resolver = new SingletonFactoryResolver(Proxy, typeof(T));
            return Add(typeof(T), contracts, resolver);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
        }

        public ContainerDescriptor AddSingleton<T>(Func<Container, T> factory)
        {
            return AddSingleton(factory, typeof(T));
        }

        public ContainerDescriptor AddTransient(Type concrete, params Type[] contracts)
        {
            return Add(concrete, contracts, new TransientTypeResolver(concrete));
        }

        public ContainerDescriptor AddTransient(Type concrete)
        {
            return AddTransient(concrete, concrete);
        }
        
        public ContainerDescriptor AddTransient(object instance, params Type[] contracts)
        {
            return Add(instance.GetType(), contracts, new TransientValueResolver(instance));
        }

        public ContainerDescriptor AddTransient(object instance)
        {
            return AddTransient(instance, instance.GetType());
        }

        public ContainerDescriptor AddTransient<T>(Func<Container, T> factory, params Type[] contracts)
        {
            var resolver = new TransientFactoryResolver(Proxy, typeof(T));
            return Add(typeof(T), contracts, resolver);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
        }

        public ContainerDescriptor AddTransient<T>(Func<Container, T> factory)
        {
            return AddTransient(factory, typeof(T));
        }

        public ContainerDescriptor AddDecorator(Type concrete, Type contract)
        {
            var count = _descriptors.Count;

            for (int i = 0; i < count; i++)
            {
                var descriptor = _descriptors[i];

                if (!descriptor.Contracts.Contains(contract))
                {
                    continue;
                }

                var decoratedContract = new DecoratedType(contract);

                object Factory(Container container)
                {
                    var dependency = container.Resolve(decoratedContract);
                    return ConstructorInjector.Construct(concrete, new[] {dependency});
                }

                var resolver = descriptor.Resolver.Lifetime == Lifetime.Singleton
                    ? (Resolver) new SingletonFactoryResolver(Factory, concrete)
                    : (Resolver) new TransientFactoryResolver(Factory, concrete);
                
                AddWithoutValidation(new[] {contract}, resolver);

                var originalDescriptorNewContracts = descriptor.Contracts.Except(new[] {contract}).Concat(new[] {decoratedContract}).ToArray();
                _descriptors[i] = new ResolverDescriptor(descriptor.Resolver, originalDescriptorNewContracts);
            }

            return this;
        }

        private void Build(out DisposableCollection disposables, out Dictionary<Type, List<Resolver>> resolversByContract, out IEnumerable<Resolver> toStart)
        {
            disposables = new DisposableCollection();
            resolversByContract = new Dictionary<Type, List<Resolver>>();

            // Copy Inherited Resolvers
            if (_parent != null)
            {
                foreach (var kvp in _parent.ResolversByContract)
                {
                    resolversByContract[kvp.Key] = kvp.Value.ToList();
                }
            }

            // Owned Resolvers
            foreach (var descriptor in _descriptors)
            {
                disposables.Add(descriptor.Resolver);

                foreach (var contract in descriptor.Contracts)
                {
                    resolversByContract.GetOrAdd(contract, _ => new List<Resolver>()).Add(descriptor.Resolver);
                }
            }

            // Non-inherited/self startables
            toStart = _descriptors.Where(d => d.Contracts.Contains(typeof(IStartable))).Select(d => d.Resolver);
        }

        public bool HasBinding(Type type)
        {
            return _descriptors.Any(descriptor => descriptor.Contracts.Contains(type));
        }

        private ContainerDescriptor Add(Type concrete, Type[] contracts, Resolver resolver)
        {
            ValidateContracts(concrete, contracts);
            var resolverDescriptor = new ResolverDescriptor(resolver, contracts);
            _descriptors.Add(resolverDescriptor);
            return this;
        }

        private ContainerDescriptor AddWithoutValidation(Type[] contracts, Resolver resolver)
        {
            var resolverDescriptor = new ResolverDescriptor(resolver, contracts);
            _descriptors.Add(resolverDescriptor);
            return this;
        }

        [Conditional("UNITY_EDITOR")]
        private static void ValidateContracts(Type concrete, params Type[] contracts)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ContractDefinitionException(concrete, contract);
                }
            }
        }
    }
}