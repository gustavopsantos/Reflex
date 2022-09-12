using UnityEngine;

namespace Reflex
{
	internal static class MonoInjectableCache
	{
		internal static readonly LazyDictionary<MonoBehaviour, MonoInjectableInfo> Cache = new LazyDictionary<MonoBehaviour, MonoInjectableInfo>(BakeMonoInjectableInfo.Bake);
	}
}