using Reflex.Core;
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    internal class ReflexSampleInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public void InstallBindings(ContainerDescriptor descriptor)
        {
            InstallInput(descriptor, useMouse: false);
            descriptor.AddSingleton(_pickupSoundEffectPrefab);
            descriptor.AddSingleton(_collectorConfigurationModel);
            descriptor.AddSingleton(typeof(CollectionStoragePrefs), typeof(ICollectionStorage));
        }

        private static void InstallInput(ContainerDescriptor descriptor, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            descriptor.AddSingleton(implementation, typeof(ICollectorInput));
        }
    }
}