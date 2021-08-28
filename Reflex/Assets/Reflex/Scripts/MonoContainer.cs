using System;
using UnityEngine;

namespace Reflex.Scripts
{
    public class MonoContainer : MonoBehaviour, IContainer
    {
        private readonly IContainer _container = new Container();

        public void Clear()
        {
            _container.Clear();
        }

        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return _container.Bind<TContract>();
        }

        public void BindSingleton<TContract>(TContract instance)
        {
            _container.BindSingleton<TContract>(instance);
        }

        public void BindSingleton<T>(Type contract, T instance)
        {
            _container.BindSingleton(contract, instance);
        }

        public BindingGenericContractDefinition BindGenericContract(Type genericContract)
        {
            return _container.BindGenericContract(genericContract);
        }

        public TContract Resolve<TContract>()
        {
            return _container.Resolve<TContract>();
        }

        public object Resolve(Type contract)
        {
            return _container.Resolve(contract);
        }

        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete)
        {
            return _container.ResolveGenericContract<TCast>(genericContract, genericConcrete);
        }
    }
}