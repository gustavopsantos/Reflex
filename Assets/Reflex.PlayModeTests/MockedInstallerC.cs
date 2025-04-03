using Reflex.Core;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerC : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.Add(Singleton.FromValue("C"));
        }
    }
}