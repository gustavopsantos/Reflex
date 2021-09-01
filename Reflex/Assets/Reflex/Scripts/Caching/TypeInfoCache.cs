using System;
using System.Linq;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;

namespace Reflex
{
	internal static class TypeInfoCache
	{
		private static readonly IReflector _reflector;
		private static readonly Dictionary<Type, TypeInfo> _registry = new Dictionary<Type, TypeInfo>();

		static TypeInfoCache()
		{
			_reflector = CreateReflector();
		}

		private static IReflector CreateReflector()
		{
			switch (RuntimeScriptingBackend.Current)
			{
				case RuntimeScriptingBackend.Backend.Undefined: throw new Exception($"Scripting backend could not be defined.");
				case RuntimeScriptingBackend.Backend.Mono: return new MonoReflector();
				case RuntimeScriptingBackend.Backend.IL2CPP: return new IL2CPPReflector();
				default: throw new ArgumentOutOfRangeException($"{RuntimeScriptingBackend.Current} scripting backend not handled.");
			}
		}

		internal static TypeInfo GetClassInfo(Type type)
		{
			if (_registry.TryGetValue(type, out var info))
			{
				return info;
			}

			// Should we add this complexity to be able to inject string?
			if (type == typeof(string))
			{
				info = new TypeInfo(Type.EmptyTypes, args => null);
				_registry.Add(type, info);
				return info;
			}

			var constructors = type.GetConstructors();
			if (constructors.Length > 0)
			{
				var constructor = constructors.MaxBy(ctor => ctor.GetParameters().Length);
				var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
				info = new TypeInfo(parameters, _reflector.GenerateGenericActivator(type, constructor, parameters));
				_registry.Add(type, info);
				return info;
			}

			// Should we add this complexity yo be able to inject value types?
			info = new TypeInfo(Type.EmptyTypes, _reflector.GenerateDefaultActivator(type));
			_registry.Add(type, info);
			return info;
		}
	}
}