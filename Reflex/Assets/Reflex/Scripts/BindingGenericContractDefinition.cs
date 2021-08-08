using System;
using System.Linq;
using System.Collections.Generic;

namespace Reflex
{
    public class BindingGenericContractDefinition
    {
        private readonly Type _genericContract;
        private readonly RegisterFunction _register;

        private BindingGenericContractDefinition()
        {
        }

        public BindingGenericContractDefinition(Type genericContract, RegisterFunction register)
        {
            _genericContract = genericContract;
            _register = register;
        }

        public BindingScopeDefinition To(params Type[] concretes)
        {
            var bindings = new List<Binding>();

            foreach (var concrete in concretes)
            {
                var genericTypes = GetGenericTypes(_genericContract, concrete);
                var contract = _genericContract.MakeGenericType(genericTypes);
                var binding = new Binding {Concrete = concrete};
                bindings.Add(binding);
                _register(contract, binding);
            }

            return new BindingScopeDefinition(bindings.ToArray());
        }

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

        private static bool TryGetTypeGenericTypesAsInterface(Type type, Type contract, out Type[] genericArguments)
        {
            var genericInterfaces = type.GetInterfaces().Where(i => i.IsGenericType);
            var contractInterface = genericInterfaces.FirstOrDefault(i => i.GetGenericTypeDefinition() == contract);
            genericArguments = contractInterface != null ? contractInterface.GetGenericArguments() : null;
            return genericArguments != null;
        }

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