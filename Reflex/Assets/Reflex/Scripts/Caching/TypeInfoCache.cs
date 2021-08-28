using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Reflex.Scripts.Utilities;
using System.Collections.Generic;

namespace Reflex
{
	internal static class TypeInfoCache
	{
		private static readonly Dictionary<Type, TypeInfo> _registry = new Dictionary<Type, TypeInfo>();

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
				var activator = CompileGenericActivator(constructor, parameters);
				info = new TypeInfo(parameters, activator);
				_registry.Add(type, info);
				return info;
			}

			// Should we add this complexity yo be able to inject value types?
			info = new TypeInfo(Type.EmptyTypes, CompileDefaultActivator(type));
			_registry.Add(type, info);
			return info;
		}

		private static DynamicObjectActivator CompileGenericActivator(ConstructorInfo constructor, Type[] parametersTypes)
		{
			var param = Expression.Parameter(typeof(object[]));
			var argumentsExpressions = new Expression[parametersTypes.Length];

			for (int i = 0; i < parametersTypes.Length; i++)
			{
				var index = Expression.Constant(i);
				var parameterType = parametersTypes[i];
				var parameterAccessor = Expression.ArrayIndex(param, index);
				var parameterCast = Expression.Convert(parameterAccessor, parameterType);
				argumentsExpressions[i] = parameterCast;
			}

			var newExpression = Expression.New(constructor, argumentsExpressions);
			var lambda = Expression.Lambda(typeof(DynamicObjectActivator), Expression.Convert(newExpression, typeof(object)), param);
			return (DynamicObjectActivator) lambda.Compile();
		}

		private static DynamicObjectActivator CompileDefaultActivator(Type type)
		{
			var body = Expression.Convert(Expression.Default(type), typeof(object));
			var lambda = Expression.Lambda(typeof(DynamicObjectActivator), body, Expression.Parameter(typeof(object[])));
			return (DynamicObjectActivator) lambda.Compile();
		}
	}
}