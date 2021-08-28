using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflex
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, Binding> _bindings = new Dictionary<Type, Binding>(); // TContract, Binding
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>(); // TContract, Instance

        public Container()
        {
            Bind<IContainer>().FromMethod(() => this);
        }

        public void Clear()
        {
            _bindings.Clear();
            _singletons.Clear();
        }
        
        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return new BindingContractDefinition<TContract>(_bindings.Add);
        }

        public void BindSingleton<TContract>(TContract instance)
        {
            _bindings.Add(typeof(TContract), new Binding
            {
                Concrete = instance.GetType(),
                Scope = BindingScope.Singleton
            });

            _singletons.Add(typeof(TContract), instance);
        }

        public void BindSingleton<T>(Type contract, T instance)
        {
            _bindings.Add(contract, new Binding
            {
                Concrete = instance.GetType(),
                Scope = BindingScope.Singleton
            });

            _singletons.Add(contract, instance);
        }

        public BindingGenericContractDefinition BindGenericContract(Type genericContract)
        {
            return new BindingGenericContractDefinition(genericContract, _bindings.Add);
        }

        public TContract Resolve<TContract>()
        {
            return (TContract) Resolve(typeof(TContract));
        }

        public object Resolve(Type contract)
        {
            if (_bindings.TryGetValue(contract, out var binding))
            {
                switch (binding.Scope)
                {
                    case BindingScope.Method: return MethodResolver.Resolve(contract, this);
                    case BindingScope.Transient: return TransientResolver.Resolve(contract, this);
                    case BindingScope.Singleton: return SingletonResolver.Resolve(contract, this);
                    default: throw new ScopeNotHandledException($"BindingScope '{binding.Scope}' not handled.");
                }
            }

            throw BuildException(contract);
        }

        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete)
        {
            var contract = genericContract.MakeGenericType(genericConcrete);
            return (TCast) Resolve(contract);
        }

        private static UnknownContractException BuildException(Type contract)
        {
            if (!contract.IsGenericType)
            {
                return new UnknownContractException($"Cannot resolve contract type '{contract}'.");
            }

            var genericContract = contract.Name.Remove(contract.Name.IndexOf('`'));
            var genericArguments = contract.GenericTypeArguments.Select(args => args.FullName);
            var commaSeparatedArguments = string.Join(", ", genericArguments);
            var message = $"Cannot resolve contract type '{genericContract}<{commaSeparatedArguments}>'.";
            return new UnknownContractException(message);
        }

        internal Type GetConcreteTypeFor(Type contract)
        {
            return _bindings[contract].Concrete;
        }

        internal object RegisterSingletonInstance(Type contract, object concrete)
        {
            _singletons.Add(contract, concrete);
            return concrete;
        }

        internal bool TryGetSingletonInstance(Type contract, out object instance)
        {
            return _singletons.TryGetValue(contract, out instance);
        }

        internal bool TryGetMethod(Type contract, out Func<object> method)
        {
            if (_bindings.TryGetValue(contract, out var binding))
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