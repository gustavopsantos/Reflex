using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    //16 bytes + 8*int[].Length
    public readonly struct BindingScopeDefinition 
    {
        //8 bytes value on x64 (instance)
        private readonly Container _container;
        //8 bytes value on x64 (instance)
        private readonly int[]     _bindings;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BindingScopeDefinition(Container container, params int[] bindings)
        {
            _container = container;
            _bindings = bindings;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AsTransient() 
        {
            for (var i = 0; i < _bindings.Length; ++i) 
            {
                var binding = _container.Bindings.GetValueByKey(_bindings[i]);
                binding.Scope = BindingScope.Transient;
                _container.Bindings.Set(_bindings[i], binding, out _);
            }
        }

        public void AsSingleton()
        {
            for (var i = 0; i < _bindings.Length; ++i) 
            {
                var binding = _container.Bindings.GetValueByKey(_bindings[i]);
                binding.Scope = BindingScope.Singleton;
                _container.Bindings.Set(_bindings[i], binding, out _);
            }
        }
    }
}