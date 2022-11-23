using Reflex.Sample.Application;
using Reflex.Scripts;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    public class ReflexSampleInstaller : Installer
    {
        [SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public override void InstallBindings(Container container)
        {
            container.BindInstance(_pickupSoundEffectPrefab);
            container.BindInstance(_collectorConfigurationModel);
            //container.BindSingleton<ICollectorInput, CollectorInputMouse>();
            container.BindSingleton<ICollectorInput, CollectorInputKeyboard>();
            container.BindSingleton<ICollectionStorage, CollectionStoragePrefs>();
        }
    }
}