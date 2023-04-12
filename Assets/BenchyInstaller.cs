using System;
using Reflex.Core;
using UnityEngine;

public class BenchyInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(27, typeof(int));
        descriptor.AddInstance("Gustavo", typeof(string));
        descriptor.AddInstance(DateTime.Now, typeof(DateTime));
    }
}