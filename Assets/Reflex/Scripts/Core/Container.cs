using System;
using UnityEngine;
using Reflex.Injectors;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;

namespace Reflex
{
    public class Container : IDisposable
    {
        public string Name { get; }
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Dictionary<Type, IResolver> _resolvers = new Dictionary<Type, IResolver>();

        public Container(string name)
        {
            Name = name;
            InjectSelf();
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        public Container Scope(string name)
        {
            var scoped = new Container(name);

            foreach (var pair in _resolvers)
            {
                scoped._resolvers[pair.Key] = pair.Value;
            }
            
            scoped.InjectSelf();
            return scoped;
        }

        private void InjectSelf()
        {
            BindInstance<Container>(this);
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.TryAdd(disposable);
        }

        public void BindFunction<TContract>(Func<TContract> function)
        {
            _resolvers.Add(typeof(TContract), new FunctionResolver(function as Func<object>));
        }
        
        public void BindInstance<TContract>(TContract instance)
        {
            _resolvers[typeof(TContract)] = new InstanceResolver(instance);
        }

        public void BindTransient<TContract, TConcrete>() where TConcrete : TContract
        {
            _resolvers[typeof(TContract)] = new TransientResolver(typeof(TConcrete));
        }

        public void BindSingleton<TContract, TConcrete>() where TConcrete : TContract
        {
            _resolvers[typeof(TContract)] = new SingletonResolver(typeof(TConcrete));
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
        
        public T Instantiate<T>(T original) where T : Component
        {
            var instance = UnityEngine.Object.Instantiate<T>(original);
            instance.GetComponentsInChildren<MonoBehaviour>().ForEach(mb => MonoInjector.Inject(mb, this));
            return instance;
        }

        public GameObject Instantiate(GameObject original)
        {
            var instance = UnityEngine.Object.Instantiate<GameObject>(original);
            instance.GetComponentsInChildren<MonoBehaviour>().ForEach(mb => MonoInjector.Inject(mb, this));
            return instance;
        }
    }
}