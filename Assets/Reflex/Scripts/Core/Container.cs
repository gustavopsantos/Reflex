using System;
using UnityEngine;
using Reflex.Scripts;
using Reflex.Injectors;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;

namespace Reflex
{
    public class Container : IContainer
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Dictionary<Type, IResolver> _resolvers = new Dictionary<Type, IResolver>();

        public Container()
        {
            InjectSelf();
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        public Container Scope()
        {
            var scoped = new Container();

            foreach (var pair in _resolvers)
            {
                scoped._resolvers[pair.Key] = pair.Value;
            }
            
            scoped.InjectSelf();
            return scoped;
        }

        private void InjectSelf()
        {
            BindSingleton<Container>(this);
            BindSingleton<IContainer>(this);
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.TryAdd(disposable);
        }

        public void BindFunction<TContract>(Func<TContract> function)
        {
            _resolvers.Add(typeof(TContract), new FunctionResolver(function as Func<object>));
        }

        public void BindTransient<TContract, TConcrete>() where TConcrete : TContract
        {
            _resolvers[typeof(TContract)] = new TransientResolver(typeof(TConcrete));
        }

        public void BindSingleton<TContract, TConcrete>() where TConcrete : TContract
        {
            _resolvers[typeof(TContract)] = new SingletonResolver(typeof(TConcrete), null);
        }

        public void BindSingleton<TContract>(TContract instance)
        {
            _resolvers[typeof(TContract)] = new SingletonResolver(instance.GetType(), instance);
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