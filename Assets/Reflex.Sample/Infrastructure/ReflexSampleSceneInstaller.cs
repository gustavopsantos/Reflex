using Reflex.Core;
using Reflex.Enums;
using Reflex.Sample.Application;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Reflex.Sample.Infrastructure
{
    internal class ReflexSampleSceneInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InstallInput(containerBuilder, useMouse: false);
            containerBuilder.RegisterValue(_collectorConfigurationModel);
            containerBuilder.RegisterType(typeof(CollectionStoragePrefs), new[] { typeof(ICollectionStorage) }, Lifetime.Singleton, Resolution.Lazy);
        }

        private static void InstallInput(ContainerBuilder containerBuilder, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            containerBuilder.RegisterType(implementation, new[] { typeof(ICollectorInput) }, Lifetime.Singleton, Resolution.Lazy);
        }
    }
}