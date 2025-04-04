using Reflex.Core;
using Reflex.Enums;
using Reflex.Sample.Infrastructure;
using UnityEngine;

public class ReflexSampleProjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(_pickupSoundEffectPrefab, Lifetime.Singleton);
    }
}