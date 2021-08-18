using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace Reflex.Scripts
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(IContainer container);
    }
}