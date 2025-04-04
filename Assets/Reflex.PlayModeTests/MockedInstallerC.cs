using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerC : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("C", Lifetime.Singleton);
        }
    }
}