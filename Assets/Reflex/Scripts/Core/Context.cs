using UnityEngine;
using System.Collections.Generic;

namespace Reflex.Scripts.Core
{
    public class Context : MonoInstaller
    {
        [field: SerializeField] public ContextKind Kind { get; private set; }
        [SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();

        public override void InstallBindings(IContainer container)
        {
            foreach (var monoInstaller in _monoInstallers)
            {
                monoInstaller.InstallBindings(container);
            }

            Debug.Log($"{Kind} Bindings Installed");
        }
    }
}