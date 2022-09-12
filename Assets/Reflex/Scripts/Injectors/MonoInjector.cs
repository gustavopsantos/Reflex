using Reflex.Scripts;
using UnityEngine;

namespace Reflex.Injectors
{
	internal static class MonoInjector
	{
		internal static void Inject(MonoBehaviour monoBehaviour, IContainer container)
		{
			var info = MonoInjectableCache.Cache[monoBehaviour];
			FieldInjector.InjectMany(info.InjectableFields, monoBehaviour, container);
			PropertyInjector.InjectMany(info.InjectableProperties, monoBehaviour, container);
			MethodInjector.InjectMany(info.InjectableMethods, monoBehaviour, container);
		}
	}
}