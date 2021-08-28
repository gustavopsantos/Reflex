using System;
using System.Linq;
using System.Reflection;
using Reflex.Scripts.Attributes;
using System.Collections.Generic;

public static class MonoInjectableTypeCache 
{
	private static readonly Dictionary<Type, MonoInjectableTypeInfo> _registry = new Dictionary<Type, MonoInjectableTypeInfo>();

	internal static MonoInjectableTypeInfo Get(Type type) 
	{
		if (_registry.TryGetValue(type, out var info))
		{
			return info;
		}
                
		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		info.InjectableFields = type.GetFields(flags).Where(f => f.IsDefined(typeof(MonoInjectAttribute))).ToArray();
		info.InjectableProperties = type.GetProperties(flags).Where(p => p.CanWrite && p.IsDefined(typeof(MonoInjectAttribute))).ToArray();
		info.InjectableMethods = type.GetMethods(flags).Where(m => m.IsDefined(typeof(MonoInjectAttribute))).ToArray();
		_registry.Add(type, info);
		
		return info;
	}
}