using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    //8 bytes
    public readonly struct BindingContractDefinition<TContract>
    {
        //do not use delegates and events, it allocates some garbage
        //do not use IContainer for internal using, it is slower than a specific type when we run on IL2CPP
        //-----------------------------------
        //8 bytes ref on x64 (instance)
        private readonly Container _container;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BindingContractDefinition(Container container)
        {
            _container = container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingScopeDefinition To<TConcrete>() where TConcrete : TContract
        {
            var hashContract = TypeHashCodeHelper<TContract>.Hash;
            
            //faster than call .cctor struct/class on IL2CPP
            Binding binding;
            binding.ConcreteHashCode = TypeHashCodeHelper<TConcrete>.Hash;
            binding.Scope = BindingScope.None;
            binding.Method = null;
            TypeInfoCache.Register<TConcrete>();

            _container.Bindings.Add(hashContract, binding, out _);
            return new BindingScopeDefinition(_container, hashContract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FromMethod(Func<TContract> method)
        {
            var hashCode = TypeHashCodeHelper<TContract>.Hash;
            
            //faster than call .cctor struct/class on IL2CPP
            Binding binding;
            binding.ConcreteHashCode = -1;
            binding.Scope            = BindingScope.Method;
            binding.Method           = method as Func<object>;

            _container.Bindings.Add(hashCode, binding, out _);
        }
    }
}