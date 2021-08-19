using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    //16 bytes
    public readonly struct BindingGenericContractDefinition
    {
        //8 bytes ref on x64 (instance)
        private readonly Type _genericContract;
        //8 bytes ref on x64 (instance)
        private readonly Container _container;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BindingGenericContractDefinition(Type genericContract, Container container)
        {
            _genericContract = genericContract;
            _container = container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingScopeDefinition To(params Type[] concretes)
        {
            var bindings = new int[concretes.Length];

            for (var i = 0; i < concretes.Length; ++i) {
                var concrete     = concretes[i];
                var genericTypes = GetGenericTypes(this._genericContract, concrete);
                var contract     = this._genericContract.MakeGenericType(genericTypes);
                var hashConcrete = contract.GetHashCode();

                Binding binding;
                binding.ConcreteHashCode = hashConcrete;
                binding.Scope = BindingScope.None;
                binding.Method = null;
                TypeInfoCache.Register(concrete);
                
                bindings[i] = hashConcrete;
                
                _container.Bindings.Add(contract.GetHashCode(), binding, out _);
            }

            return new BindingScopeDefinition(_container, bindings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type[] GetGenericTypes(Type genericContract, Type type)
        {
            if (TryGetTypeGenericTypesAsInterface(type, genericContract, out var interfaceGenericArguments))
            {
                return interfaceGenericArguments;
            }

            if (TryGetTypeGenericTypesAsAbstract(type, genericContract, out var abstractGenericArguments))
            {
                return abstractGenericArguments;
            }

            throw new Exception($"Could not retrieve generic types from type '{type.Name}'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetTypeGenericTypesAsInterface(Type type, Type contract, out Type[] genericArguments)
        {
            var genericInterfaces = type.GetInterfaces().Where(i => i.IsGenericType);
            var contractInterface = genericInterfaces.FirstOrDefault(i => i.GetGenericTypeDefinition() == contract);
            genericArguments = contractInterface != null ? contractInterface.GetGenericArguments() : null;
            return genericArguments != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetTypeGenericTypesAsAbstract(Type type, Type contract, out Type[] genericArguments)
        {
            genericArguments = type.BaseType != null &&
                               type.BaseType.IsGenericType &&
                               type.BaseType.GetGenericTypeDefinition() == contract
                ? type.BaseType.GetGenericArguments()
                : null;

            return genericArguments != null;
        }
    }
}