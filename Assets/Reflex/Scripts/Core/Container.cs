using System;
using UnityEngine;
using Reflex.Injectors;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;
using Reflex.Scripts.Enums;
using Reflex.Scripts.Extensions;

namespace Reflex
{
    public class Container : IDisposable
    {
        public Container Parent { get; private set; }
        public IReadOnlyList<Container> Children => _children;

        public string Name { get; }
        private readonly List<Container> _children = new List<Container>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        internal readonly Dictionary<Type, IResolver> _resolvers = new Dictionary<Type, IResolver>();

        public Container(string name)
        {
            Name = name;
            InjectSelf();
        }

        public void Dispose()
        {
            foreach (var child in _children.Reversed())
            {
                child.Dispose();
            }

            Debug.Log($"Disposing container: {Name}");
            _disposables.Dispose();

            if (Parent != null)
            {
                Parent._children.Remove(this);
                Parent = null;
            }
        }

        public Container Scope(string name)
        {
            var scoped = new Container(name);
            scoped.Parent = this;
            _children.Add(scoped);

            foreach (var pair in _resolvers)
            {
                scoped._resolvers[pair.Key] = pair.Value;
            }

            scoped.InjectSelf();
            return scoped;
        }

        private void InjectSelf()
        {
            BindInstance(this);
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.TryAdd(disposable);
        }

        public bool Contains(Type type)
        {
            return _resolvers.ContainsKey(type);
        }

        public void BindFunction<TContract>(Func<TContract> function)
        {
            var resolver = new FunctionResolver(function as Func<object>);
            _disposables.Add(resolver);
            _resolvers.Add(typeof(TContract), resolver);
        }

        public void BindInstance(object instance)
        {
            BindInstanceAs(instance, instance.GetType());
        }

        public void BindInstanceAs<TContract>(TContract instance)
        {
            BindInstanceAs(instance, typeof(TContract));
        }

        public void BindInstanceAs(object instance, Type asType)
        {
            var resolver = new InstanceResolver(instance);
            _disposables.Add(resolver);
            _resolvers[asType] = resolver;
        }

        public void BindTransient<TContract, TConcrete>() where TConcrete : TContract
        {
            var resolver = new TransientResolver(typeof(TConcrete));
            _disposables.Add(resolver);
            _resolvers[typeof(TContract)] = resolver;
        }

        public void BindSingleton<TContract, TConcrete>() where TConcrete : TContract
        {
            var resolver = new SingletonResolver(typeof(TConcrete));
            _disposables.Add(resolver);
            _resolvers[typeof(TContract)] = resolver;
        }

        public T Construct<T>()
        {
            return ConstructorInjector.ConstructAndInject<T>(this);
        }

        public object Construct(Type concrete)
        {
            return ConstructorInjector.ConstructAndInject(concrete, this);
        }

        public TContract Resolve<TContract>()
        {
            return (TContract) Resolve(typeof(TContract));
        }

        public object Resolve(Type contract)
        {
            if (_resolvers.TryGetValue(contract, out var resolver))
            {
                return resolver.Resolve(this);
            }

            throw new UnknownContractException(contract);
        }

        public void InjectMono(Component instance, MonoInjectionMode injectionMode = MonoInjectionMode.Single)
        {
            instance.GetInjectables(injectionMode).ForEach(mb => MonoInjector.Inject(mb, this));
        }

        public T Instantiate<T>(T original, Transform container = null,
            MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) where T : Component
        {
            return MonoInstantiate.Instantiate(original, container, this,
                (parent) => UnityEngine.Object.Instantiate<T>(original, parent), injectionMode);
        }

        public T Instantiate<T>(T original, Transform container, bool worldPositionStays,
            MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) where T : Component
        {
            return MonoInstantiate.Instantiate(original, container, this,
                (parent) => UnityEngine.Object.Instantiate<T>(original, parent, worldPositionStays), injectionMode);
        }

        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform container = null,
            MonoInjectionMode injectionMode = MonoInjectionMode.Recursive) where T : Component
        {
            return MonoInstantiate.Instantiate(original, container, this,
                (parent) => UnityEngine.Object.Instantiate<T>(original, position, rotation, parent), injectionMode);
        }
    }
}