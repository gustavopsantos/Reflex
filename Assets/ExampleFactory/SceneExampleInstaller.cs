using System.Collections;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

public class SceneExampleInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton("World");
    }
}
