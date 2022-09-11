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
        internal readonly CompositeDisposable Disposables = new CompositeDisposable();
        internal readonly Dictionary<Type, Resolver> Resolvers = new Dictionary<Type, Resolver>();

        public Container()
        {
            Resolvers[typeof(IContainer)] = new SingletonResolver(typeof(Container), this);
        }
        
        public Container Scope()
        {
            var scoped = new Container();

            foreach (var pair in Resolvers)
            {
                scoped.Resolvers[pair.Key] = pair.Value;
            }

            return scoped;
        }

        public void AddDisposable(IDisposable disposable)
        {
            Disposables.TryAdd(disposable);
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

        public T Construct<T>()
        {
            return ConstructorInjector.ConstructAndInject<T>(this);
        }
        
        public object Construct(Type concrete)
        {
            return ConstructorInjector.ConstructAndInject(concrete, this);
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }

        public void BindFunction<TContract>(Func<TContract> function)
        {
            Resolvers.Add(typeof(TContract), new FunctionResolver(function as Func<object>));
        }

        public void BindTransient<TContract, TConcrete>() where TConcrete : TContract
        {
            Resolvers[typeof(TContract)] = new TransientResolver(typeof(TConcrete));
        }

        public void BindSingleton<TContract, TConcrete>() where TConcrete : TContract
        {
            Resolvers[typeof(TContract)] = new SingletonResolver(typeof(TConcrete), null);
        }

        public void BindSingleton<TContract>(TContract instance)
        {
            Resolvers[typeof(TContract)] = new SingletonResolver(instance.GetType(), instance);
        }

        public TContract Resolve<TContract>()
        {
            return (TContract) Resolve(typeof(TContract));
        }

        public object Resolve(Type contract)
        {
            if (Resolvers.TryGetValue(contract, out var resolver))
            {
                return resolver.Resolve(this);
            }

            throw new UnknownContractException(contract);
        }
    }
}