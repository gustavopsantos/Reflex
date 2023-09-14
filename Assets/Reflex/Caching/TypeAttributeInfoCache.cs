using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;

namespace Reflex.Caching
{
	internal static class TypeAttributeInfoCache
	{
		private const BindingFlags _flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private static readonly Dictionary<Type, TypeAttributeInfo> _dictionary = new();

		internal static TypeAttributeInfo Get(Type type)
		{
			if (_dictionary.TryGetValue(type, out TypeAttributeInfo info))
			{
				return info;
			}

			info = Generate(type);
			_dictionary.Add(type, info);

			return info;
		}

		private static TypeAttributeInfo Generate(Type type)
		{
			FieldInfo[] fields = type
				.GetFields(_flags)
				.Where(f => f.IsDefined(typeof(InjectAttribute)))
				.ToArray();

			PropertyInfo[] properties = type
				.GetProperties(_flags)
				.Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)))
				.ToArray();

			MethodInfo[] methods = type
				.GetMethods(_flags)
				.Where(m => m.IsDefined(typeof(InjectAttribute)))
				.ToArray();

			return new TypeAttributeInfo(fields, properties, methods);
		}
	}
}