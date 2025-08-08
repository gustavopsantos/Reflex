using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Exceptions;
using Reflex.Generics;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class ContainerBuilder
    {
        public string Name { get; private set; }
        public List<Container> Parents { get; } = new();
        public List<Binding> Bindings { get; } = new();
        public event Action<Container> OnContainerBuilt;
        
        private static readonly Dictionary<Type, HashSet<IResolver>> ResolversByContract = new();

        public Container Build()
        {
            var disposables = new DisposableCollection();
            ResolversByContract.Clear();

            // Inherited resolvers
            foreach (var parent in Parents)
            {
                foreach (var (contract, parentResolvers) in parent.ResolversByContract)
                {
                    if (!ResolversByContract.TryGetValue(contract, out var resolversSet))
                    {
                        resolversSet = new(parentResolvers);
                        ResolversByContract[contract] = resolversSet;
                    } 
                    else
                    {
                        resolversSet.UnionWith(parentResolvers);
                    }
                }
            }

            // Owned resolvers
            foreach (var binding in Bindings)
            {
                disposables.Add(binding.Resolver);

                foreach (var contract in binding.Contracts)
                {
                    if (!ResolversByContract.TryGetValue(contract, out var resolversSet))
                    {
                        resolversSet = new ();
                        ResolversByContract[contract] = resolversSet;
                    }
                    resolversSet.Add(binding.Resolver);
                }
            }
            
            Bindings.Clear();   
            var container = new Container(Name, Parents, 
                ResolversByContract.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()),
                disposables);
            OnContainerBuilt?.Invoke(container);
            return container;
        }

        public ContainerBuilder SetName(string name)
        {
            Name = name;
            return this;
        }
        
        public ContainerBuilder AddParent(Container parent)
        {
            if (parent == null)
                throw new ContainerBuilderAddNullParentException(this);
            Parents.Add(parent);
            return this;
        }

        public ContainerBuilder RemoveParent(Container parent)
        {
            Parents.Remove(parent);
            return this;
        }
        
        public ContainerBuilder RemoveAllParents()
        {
            Parents.Clear();
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