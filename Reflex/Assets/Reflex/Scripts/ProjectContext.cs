using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Reflex.Scripts
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class ProjectContext : MonoContainer
    {
        [SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InstallBindings()
        {
            foreach (var monoInstaller in _monoInstallers)
            {
                monoInstaller.InstallBindings(this);
            }
        }
    }
}