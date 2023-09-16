using Microsoft.Extensions.DependencyInjection;
using Reflex.Microsoft.Core;
using Reflex.Microsoft.Sample.Application;
using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
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

	internal class ReflexMicrosoftInstaller : MonoBehaviour, IInstaller
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