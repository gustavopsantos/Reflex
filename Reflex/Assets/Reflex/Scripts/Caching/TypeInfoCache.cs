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

		// Expressions for Mono Runtime, and FormatterServices for IL2CPP
		internal static TypeInfo GetClassInfo(Type type)
		{
			DynamicObjectActivator activator;
			
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
#if ENABLE_IL2CPP
                activator = args => 
                {
                    var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                    constructor.Invoke(instance, args);
                    return instance;
                };
#else
				activator = CompileGenericActivator(constructor, parameters);
#endif
				info = new TypeInfo(parameters, activator);
				_registry.Add(type, info);
				return info;
			}

			// Should we add this complexity yo be able to inject value types?
#if ENABLE_IL2CPP
			activator = args => System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
			activator = CompileDefaultActivator(type);
#endif
			
			info = new TypeInfo(Type.EmptyTypes, activator);
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