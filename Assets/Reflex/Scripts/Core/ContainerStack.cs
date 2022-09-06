using System;
using UnityEngine;
using Reflex.Injectors;
using System.Collections.Generic;

namespace Reflex.Scripts.Core
{
    public class ContainerStack : IContainer
    {
        private readonly Stack<Container> _stack = new Stack<Container>();

        internal Container PushNew()
        {
            var container = new Container();
            container.BindSingleton<IContainer>(this);
            _stack.Push(container);
            return container;
        }
        
        internal Container Pop()
        {
            return _stack.Pop();
        }
        
        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                _stack.Pop().Dispose();
            }
        }

        public void AddDisposable(IDisposable disposable)
        {
            _stack.Peek().AddDisposable(disposable);
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

        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return _stack.Peek().Bind<TContract>();
        }
        
        public TContract Resolve<TContract>()
        {
            return (TContract) Resolve(typeof(TContract));
        }

        public object Resolve(Type contract)
        {
            foreach (var container in _stack)
            {
                try
                {
                    var instance = container.Resolve(contract);
                    if (instance != null)
                    {
                        return instance;
                    }
                }
                catch (Exception)
                {
                }
            }

            throw new Exception("Could not resolve");
        }

        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete)
        {
            var contract = genericContract.MakeGenericType(genericConcrete);
            return (TCast) Resolve(contract);
        }

        public void BindSingleton<TContract>(TContract instance)
        {
            _stack.Peek().BindSingleton<TContract>(instance);
        }
    }
}