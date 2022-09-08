using UnityEngine;
using System.Collections.Generic;

namespace Reflex.Scripts.Core
{
    public abstract class AContext : MonoInstaller
    {
        [SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();

        public override void InstallBindings(IContainer container)
        {
            foreach (var monoInstaller in _monoInstallers)
            {
                monoInstaller.InstallBindings(container);
            }
        }
    }
}