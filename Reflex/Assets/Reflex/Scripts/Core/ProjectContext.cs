using System.Linq;
using UnityEngine;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;

namespace Reflex.Scripts
{
	public class ProjectContext : MonoContainer
	{
		[SerializeField] private List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();

		public void InstallBindings()
		{
			foreach (var monoInstaller in _monoInstallers)
			{
				monoInstaller.InstallBindings(Container);
			}
		}

		public void InstantiateNonLazySingletons()
		{
			Container.SingletonNonLazyResolver = new SingletonLazyResolver();
			Container.Bindings.Values.Where(IsSingletonNonLazy).ForEach(binding => Container.Resolve(binding.Contract));
			Container.SingletonNonLazyResolver = new SingletonNonLazyResolver();
		}

		private static bool IsSingletonNonLazy(Binding binding)
		{
			return binding.Scope == BindingScope.SingletonNonLazy;
		}
	}
}