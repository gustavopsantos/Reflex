using System;
using System.Collections.Generic;
using System.Linq;
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
        public static Container ProjectContainer { get; internal set; } 
        public string Name { get; }
        public List<Container> Parents { get; }
        internal List<Container> Children { get; } = new();
        internal Dictionary<Type, List<IResolver>> ResolversByContract { get; }
        internal DisposableCollection Disposables { get; }
#if UNITY_EDITOR
        internal static readonly List<Container> RootContainers = new();
#endif
        
        internal Container(string name, List<Container> parents, Dictionary<Type, List<IResolver>> resolversByContract, DisposableCollection disposables)
        {
            Diagnosis.RegisterBuildCallSite(this);
            Name = name;
            Parents = parents;
            foreach (var parent in Parents)
            {
                parent.Children.Add(this);
            }
            ResolversByContract = resolversByContract;
            Disposables = disposables;
            OverrideSelfInjection();

#if UNITY_EDITOR
            if (Parents.Count == 0)
            {
                RootContainers.Add(this);
            }
#endif
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

            foreach (var parent in Parents)
            {
                parent.Children.Remove(this);
            }

#if UNITY_EDITOR
            RootContainers.Remove(this);
#endif
            
            ResolversByContract.Clear();
            Disposables.Dispose();
            ReflexLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(Action<ContainerBuilder> extend = null)
        {
            var builder = new ContainerBuilder().AddParent(this);
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
                ? resolvers.Select(resolver => resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<object>();
        }

        public IEnumerable<TContract> All<TContract>()
        {
            return ResolversByContract.TryGetValue(typeof(TContract), out var resolvers)
                ? resolvers.Select(resolver => (TContract) resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<IResolver> GetResolvers(Type contract)
        {
            if (ResolversByContract.TryGetValue(contract, out var resolvers))
            {
                return resolvers;
            }

            throw new UnknownContractException(contract);
        }
        
        private void OverrideSelfInjection()
        {
            ResolversByContract[typeof(Container)] = new List<IResolver> { new SingletonValueResolver(this) };
        }
    }
}