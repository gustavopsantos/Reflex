using System;

namespace Reflex
{
    public interface IContainer
    {
        void Clear();
        BindingContractDefinition<TContract> Bind<TContract>();
        void BindSingleton<TContract>(TContract instance);
        void BindSingleton<T>(Type contract, T instance);
        BindingGenericContractDefinition BindGenericContract(Type genericContract);
        TContract Resolve<TContract>();
        object Resolve(Type contract);
        TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete);
    }
}