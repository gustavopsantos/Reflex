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

            var constructor = FindMostValuableConstructor(type);
            var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var activator = CompileGenericActivator(constructor, parameters);
            info = new TypeInfo(parameters, activator);
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

        private static ConstructorInfo FindMostValuableConstructor(Type concrete)
        {
            return concrete.GetConstructors().MaxBy(constructor => constructor.GetParameters().Length);
        }
    }
}