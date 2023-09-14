using Microsoft.Extensions.DependencyInjection;
using Reflex.Core;
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
	public static class Extensions
	{
		public static IServiceCollection InstallInput(this IServiceCollection serviceCollection, bool useMouse)
		{
			return serviceCollection.AddSingleton<ICollectorInput>(serviceProvider =>
				useMouse
					? new CollectorInputMouse()
					: new CollectorInputKeyboard());
		}
	}

	internal class ReflexSampleInstaller : MonoBehaviour, IInstaller
	{
		[SerializeField] private PickupSoundEffect _pickupSoundEffectPrefab;
		[SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

		public void InstallBindings(IServiceCollection serviceCollection)
		{
			serviceCollection
				.InstallInput(false)
				.AddSingleton(_pickupSoundEffectPrefab)
				.AddSingleton(_collectorConfigurationModel)
				.AddSingleton<ICollectionStorage, CollectionStoragePrefs>();
		}
	}
}