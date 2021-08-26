using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class Container : IContainer
    {
        internal readonly IntHashMap<Binding> Bindings = new IntHashMap<Binding>(); // TContract, Binding
        internal readonly IntHashMap<object> Singletons = new IntHashMap<object>(); // TContract, Instance

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Container()
        {
            Bind<IContainer>().FromMethod(() => this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Bindings.Clear();
            Singletons.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContractDefinition<TContract> Bind<TContract>()
        {
            return new BindingContractDefinition<TContract>(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSingleton<TContract>(TContract instance)
        {
            var hashContract = TypeHashCodeHelper<TContract>.Hash;
            var typeConcrete = instance.GetType();

            //faster than call .cctor struct/class on IL2CPP
            Binding binding;
            binding.ConcreteHashCode = typeConcrete.GetHashCode();
            binding.Scope = BindingScope.Singleton;
            binding.Method = null;
            TypeInfoCache.Register(typeConcrete);

            Bindings.Add(hashContract, binding, out _);
            Singletons.Add(hashContract, instance, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSingleton<T>(Type contract, T instance)
        {
            var hashContract = contract.GetHashCode();

            //faster than call .cctor struct/class on IL2CPP
            Binding binding;
            binding.ConcreteHashCode = TypeHashCodeHelper<T>.Hash;
            binding.Scope = BindingScope.Singleton;
            binding.Method = null;
            TypeInfoCache.Register<T>();

            Bindings.Add(hashContract, binding, out _);
            Singletons.Add(hashContract, instance, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingGenericContractDefinition BindGenericContract(Type genericContract)
        {
            return new BindingGenericContractDefinition(genericContract, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TContract Resolve<TContract>()
        {
            var hashContract = TypeHashCodeHelper<TContract>.Hash;

            if (Bindings.TryGetValue(hashContract, out var binding))
            {
                switch (binding.Scope)
                {
                    case BindingScope.Method: return (TContract)MethodResolver.Resolve(hashContract, this);
                    case BindingScope.Transient: return (TContract)TransientResolver.Resolve(hashContract, this);
                    case BindingScope.Singleton: return (TContract)SingletonResolver.Resolve(hashContract, this);
                    default: throw new ScopeNotHandledException($"BindingScope '{binding.Scope}' not handled.");
                }
            }

            throw BuildException(typeof(TContract));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type contract)
        {
            var hashContract = contract.GetHashCode();

            if (Bindings.TryGetValue(hashContract, out var binding))
            {
                switch (binding.Scope)
                {
                    case BindingScope.Method: return MethodResolver.Resolve(hashContract, this);
                    case BindingScope.Transient: return TransientResolver.Resolve(hashContract, this);
                    case BindingScope.Singleton: return SingletonResolver.Resolve(hashContract, this);
                    default: throw new ScopeNotHandledException($"BindingScope '{binding.Scope}' not handled.");
                }
            }

            throw BuildException(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCast ResolveGenericContract<TCast>(Type genericContract, params Type[] genericConcrete)
        {
            var contract = genericContract.MakeGenericType(genericConcrete);
            return (TCast)Resolve(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}
