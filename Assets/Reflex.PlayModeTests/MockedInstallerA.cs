using Reflex.Core;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerA : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("A");
        }
    }
}