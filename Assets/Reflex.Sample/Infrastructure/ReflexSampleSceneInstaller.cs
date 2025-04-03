using Reflex.Core;
using Reflex.Sample.Application;
using UnityEngine;
using Resolution = Reflex.Core.Resolution;

namespace Reflex.Sample.Infrastructure
{
    internal class ReflexSampleSceneInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InstallInput(containerBuilder, useMouse: false);
            containerBuilder.Add(Singleton.FromValue(_collectorConfigurationModel));
            containerBuilder.Add(Singleton.FromType(typeof(CollectionStoragePrefs), new[] { typeof(ICollectionStorage) }, Resolution.Lazy));
        }

        private static void InstallInput(ContainerBuilder containerBuilder, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            containerBuilder.Add(Singleton.FromType(implementation, new[] { typeof(ICollectorInput) }, Resolution.Lazy));
        }
    }
}