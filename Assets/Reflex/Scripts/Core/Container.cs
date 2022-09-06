using System;
using System.Linq;
using System.Collections.Generic;
using Reflex.Injectors;
using Reflex.Scripts;
using Reflex.Scripts.Utilities;
using UnityEngine;

namespace Reflex
{
    public class Container : IContainer
    {
        internal readonly CompositeDisposable Disposables = new CompositeDisposable();
        internal readonly Dictionary<Type, Binding> Bindings = new Dictionary<Type, Binding>(); // TContract, Binding
        internal readonly Dictionary<Type, object> Singletons = new Dictionary<Type, object>(); // TContract, Instance

        private readonly Resolver _methodResolver = new MethodResolver();
        private readonly Resolver _transientResolver = new TransientResolver();
        private readonly Resolver _singletonResolver = new SingletonResolver();

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
            var binding = new Binding
            {
                Scope = BindingScope.Method,
                Method = function as Func<object>,
            };

            Bindings.Add(typeof(TContract), binding);
        }

        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return new BindingContractDefinition<TContract>(this);
        }
        
        public void BindSingleton<TContract>(TContract instance)
        {
            var binding = new Binding
            {
                Contract = typeof(TContract),
                Concrete = instance.GetType(),
                Scope = BindingScope.Singleton
            };

            Bindings[typeof(TContract)] = binding;
            Singletons[typeof(TContract)] = instance;
        }

        public void BindSingleton<T>(Type contract, T instance)
        {
            var binding = new Binding
            {
                Contract = contract,
                Concrete = instance.GetType(),
                Scope = BindingScope.Singleton
            };
            
            Bindings.Add(contract, binding);
            Singletons.Add(contract, instance);
        }

        public TContract Resolve<TContract>()
        {
            return (TContract) Resolve(typeof(TContract));
        }

        public object Resolve(Type contract)
        {
            var resolver = MatchResolver(contract);
            return resolver.Resolve(contract, this);
        }

        private Resolver MatchResolver(Type contract)
        {
            if (Bindings.TryGetValue(contract, out var binding))
            {
                switch (binding.Scope)
                {
                    case BindingScope.Method: return _methodResolver;
                    case BindingScope.Transient: return _transientResolver;
                    case BindingScope.Singleton: return _singletonResolver;
                    default: throw new BindingScopeNotHandledException(binding.Scope);
                }
            }

            throw new UnknownContractException(contract);
        }

        internal Type GetConcreteTypeFor(Type contract)
        {
            return Bindings[contract].Concrete;
        }

        internal object RegisterSingletonInstance(Type contract, object concrete)
        {
            Singletons.Add(contract, concrete);
            return concrete;
        }

        internal bool TryGetMethod(Type contract, out Func<object> method)
        {
            if (Bindings.TryGetValue(contract, out var binding))
            {
                if (binding.Scope == BindingScope.Method)
                {
                    method = binding.Method;
                    return true;
                }
            }

            method = null;
            return false;
        }
    }
}