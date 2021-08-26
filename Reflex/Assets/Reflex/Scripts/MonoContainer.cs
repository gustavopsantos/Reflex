using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex.Scripts
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class MonoContainer : MonoBehaviour, IContainer
    {
        private readonly IContainer _container = new Container();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _container.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return _container.Bind<TContract>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSingleton<TContract>(TContract instance)
        {
            _container.BindSingleton(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSingleton<T>(Type contract, T instance)
        {
            _container.BindSingleton(contract, instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingGenericContractDefinition BindGenericContract(Type genericContract)
        {
            return _container.BindGenericContract(genericContract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TContract Resolve<TContract>() 
        {
            return _container.Resolve<TContract>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type contract)
        {
            return _container.Resolve(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete)
        {
            return _container.ResolveGenericContract<TCast>(genericContract, genericConcrete);
        }
    }
}