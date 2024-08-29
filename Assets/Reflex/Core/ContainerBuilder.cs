using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Resolvers;

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

            // Inherited resolvers
            if (Parent != null)
            {
                foreach (var kvp in Parent.ResolversByContract)
                {
                    resolversByContract[kvp.Key] = kvp.Value.ToList();
                }
            }

            // Owned resolvers
            foreach (var binding in Bindings)
            {
                disposables.Add(binding.Resolver);

                foreach (var contract in binding.Contracts)
                {
                    resolversByContract.GetOrAdd(contract, _ => new List<IResolver>()).Add(binding.Resolver);
                }
            }

            var container = new Container(Name, Parent, resolversByContract, disposables);
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

        public ContainerBuilder AddSingleton(Type concrete, params Type[] contracts)
        {
            return Add(concrete, contracts, new SingletonTypeResolver(concrete));
        }

        public ContainerBuilder AddSingleton(Type concrete)
        {
            return AddSingleton(concrete, concrete);
        }

        public ContainerBuilder AddSingleton(object instance, params Type[] contracts)
        {
            return Add(instance.GetType(), contracts, new SingletonValueResolver(instance));
        }

        public ContainerBuilder AddSingleton(object instance)
        {
            return AddSingleton(instance, instance.GetType());
        }

        public ContainerBuilder AddSingleton<T>(Func<Container, T> factory, params Type[] contracts)
        {
            var resolver = new SingletonFactoryResolver(Proxy);
            return Add(typeof(T), contracts, resolver);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
        }

        public ContainerBuilder AddSingleton<T>(Func<Container, T> factory)
        {
            return AddSingleton(factory, typeof(T));
        }

        public ContainerBuilder AddTransient(Type concrete, params Type[] contracts)
        {
            return Add(concrete, contracts, new TransientTypeResolver(concrete));
        }

        public ContainerBuilder AddTransient(Type concrete)
        {
            return AddTransient(concrete, concrete);
        }

        public ContainerBuilder AddTransient<T>(Func<Container, T> factory, params Type[] contracts)
        {
            var resolver = new TransientFactoryResolver(Proxy);
            return Add(typeof(T), contracts, resolver);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
        }

        public ContainerBuilder AddTransient<T>(Func<Container, T> factory)
        {
            return AddTransient(factory, typeof(T));
        }
        
        // Scoped
        
        public ContainerBuilder AddScoped(Type concrete, params Type[] contracts)
        {
            return Add(concrete, contracts, new ScopedTypeResolver(concrete));
        }

        public ContainerBuilder AddScoped(Type concrete)
        {
            return AddScoped(concrete, concrete);
        }

        public ContainerBuilder AddScoped<T>(Func<Container, T> factory, params Type[] contracts)
        {
            var resolver = new ScopedFactoryResolver(Proxy);
            return Add(typeof(T), contracts, resolver);

            object Proxy(Container container)
            {
                return factory.Invoke(container);
            }
        }

        public ContainerBuilder AddScoped<T>(Func<Container, T> factory)
        {
            return AddScoped(factory, typeof(T));
        }

        public bool HasBinding(Type type)
        {
            return Bindings.Any(binding => binding.Contracts.Contains(type));
        }

        private ContainerBuilder Add(Type concrete, Type[] contracts, IResolver resolver)
        {
            var binding = Binding.Validated(resolver, concrete, contracts);
            Bindings.Add(binding);
            return this;
        }
    }
}