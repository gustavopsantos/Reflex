using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerA : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("A", Lifetime.Singleton);
        }
    }
}