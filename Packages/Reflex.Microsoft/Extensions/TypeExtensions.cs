using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflex.Microsoft.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsEnumerable(this Type type, out Type elementType)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				elementType = type.GenericTypeArguments.Single();
				return true;
			}

			elementType = default;
			return false;
		}

		public static bool TryGetConstructors(this Type type, out ConstructorInfo[] constructors)
		{
			constructors = type.GetConstructors();
			return constructors.Length > 0;
		}

		public static string GetName(this Type type)
		{
			if (type.IsGenericType)
			{
				string outerTypeName = type.Name!.Split('`').First();
				string innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
				return $"{outerTypeName}<{innerTypeNames}>";
			}

			return type.Name;
		}

		public static string GetFullName(this Type type)
		{
			if (type.IsGenericType)
			{
				string outerTypeName = type.FullName!.Split('`').First();
				string innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
				return $"{outerTypeName}<{innerTypeNames}>";
			}

			return type.FullName;
		}
	}
}