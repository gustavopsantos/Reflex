using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Reflex.Exceptions;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Logging;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Container : IDisposable
    {
        private readonly DisposableCollection _disposables;

        public string Name { get; }
        internal Container Parent { get; private set; }
        internal List<Container> Children { get; } = new();
        internal Dictionary<Type, List<Resolver>> ResolversByContract { get; }
        
        internal Container(string name, Dictionary<Type, List<Resolver>> resolversByContract, DisposableCollection disposables)
        {
            Name = name;
            ResolversByContract = resolversByContract;
            _disposables = disposables;
            OverrideSelfInjection();
        }

        public bool HasBinding<T>()
        {
            return HasBinding(typeof(T));
        }

        public bool HasBinding(Type type)
        {
            return ResolversByContract.ContainsKey(type);
        }

        public void Dispose()
        {
            foreach (var child in Children.Reversed())
            {
                child.Dispose();
            }
            
            SetParent(null);
            ResolversByContract.Clear();
            _disposables.Dispose();
            ReflexLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(string name, Action<ContainerDescriptor> extend = null)
        {
            var builder = new ContainerDescriptor(name, this);
            extend?.Invoke(builder);
            return builder.Build();
        }
        
        public T Construct<T>()
        {
            return (T)Construct(typeof(T));
        }

        public object Construct(Type concrete)
        {
            var instance = ConstructorInjector.Construct(concrete, this);
            AttributeInjector.Inject(instance, this);   
            return instance;
        }
        
        public object Resolve(Type type)
        {
            if (type.IsEnumerable(out var elementType))
            {
                return All(elementType).CastDynamic(elementType);
            }

            var resolvers = GetResolvers(type);
            var lastResolver = resolvers.Last();
            var resolved = lastResolver.Resolve(this);
            return resolved;
        }

        public TContract Resolve<TContract>()
        {
            return (TContract)Resolve(typeof(TContract));
        }
        
        public object Single(Type type)
        {
            return GetResolvers(type).Single().Resolve(this);
        }

        public TContract Single<TContract>()
        {
            return (TContract)Single(typeof(TContract));
        }

        public IEnumerable<object> All(Type contract)
        {
            return ResolversByContract.TryGetValue(contract, out var resolvers)
                ? resolvers.Select(resolver => resolver.Resolve(this))
                : Enumerable.Empty<object>();
        }

        public IEnumerable<TContract> All<TContract>()
        {
            return ResolversByContract.TryGetValue(typeof(TContract), out var resolvers)
                ? resolvers.Select(resolver => (TContract) resolver.Resolve(this))
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<Resolver> GetResolvers(Type contract)
        {
            if (ResolversByContract.TryGetValue(contract, out var resolvers))
            {
                return resolvers;
            }

            throw new UnknownContractException(contract);
        }
        
        private void OverrideSelfInjection()
        {
            ResolversByContract[typeof(Container)] = new List<Resolver> { new SingletonValueResolver(this) };
        }

        internal void SetParent([CanBeNull] Container parent)
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
                Parent = null;
            }

            if (parent != null)
            {
                Parent = parent;
                parent.Children.Add(this);
            }
        }
    }
}