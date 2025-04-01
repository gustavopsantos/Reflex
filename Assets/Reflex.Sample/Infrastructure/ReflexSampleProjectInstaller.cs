using System.Collections;
using System.Collections.Generic;
using Reflex.Core;
using Reflex.Sample.Infrastructure;
using UnityEngine;

public class ReflexSampleProjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_pickupSoundEffectPrefab);
    }
}