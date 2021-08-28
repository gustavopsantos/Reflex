using System.Collections.Generic;
using UnityEngine;

namespace Reflex.Scripts
{
    public class ProjectContext : MonoContainer
    {
        [SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();

        public void InstallBindings()
        {
            foreach (var monoInstaller in _monoInstallers)
            {
                monoInstaller.InstallBindings(this);
            }
        }
    }
}