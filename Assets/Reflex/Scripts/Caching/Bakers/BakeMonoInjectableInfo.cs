using System.Linq;
using System.Reflection;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Reflex
{
	internal static class BakeMonoInjectableInfo
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		internal static MonoInjectableInfo Bake(MonoBehaviour monoBehaviour)
		{
			var type = monoBehaviour.GetType();
			var fields = type.GetFields(Flags).Where(f => f.IsDefined(typeof(InjectAttribute))).ToArray();
			var properties = type.GetProperties(Flags).Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute))).ToArray();
			var methods = type.GetMethods(Flags).Where(m => m.IsDefined(typeof(InjectAttribute))).ToArray();
			return new MonoInjectableInfo(fields, properties, methods);
		}
	}
}