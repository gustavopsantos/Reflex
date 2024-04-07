using Reflex.Core;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerB : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton("B");
        }
    }
}