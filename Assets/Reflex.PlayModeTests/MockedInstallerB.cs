﻿using Reflex.Core;
using Reflex.Enums;
using UnityEngine;

namespace Reflex.PlayModeTests
{
    public class MockedInstallerB : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("B", Lifetime.Singleton);
        }
    }
}