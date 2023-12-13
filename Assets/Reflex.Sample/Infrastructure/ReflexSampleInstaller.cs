using Reflex.Core;
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    internal class ReflexSampleInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InstallInput(containerBuilder, useMouse: false);
            containerBuilder.AddSingleton(_pickupSoundEffectPrefab);
            containerBuilder.AddSingleton(_collectorConfigurationModel);
            containerBuilder.AddSingleton(typeof(CollectionStoragePrefs), typeof(ICollectionStorage));
        }

        private static void InstallInput(ContainerBuilder containerBuilder, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            containerBuilder.AddSingleton(implementation, typeof(ICollectorInput));
        }
    }
}