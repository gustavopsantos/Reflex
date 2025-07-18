using System;
using System.Linq.Expressions;
using System.Reflection;
using Reflex.Delegates;

namespace Reflex.Reflectors
{
    internal sealed class MonoActivatorFactory : IActivatorFactory
    {
        public ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters)
        {
            var param = Expression.Parameter(typeof(object[]));
            var argumentsExpressions = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var parameterType = parameters[i];
                var parameterAccessor = Expression.ArrayIndex(param, index);
                var parameterCast = Expression.Convert(parameterAccessor, parameterType);
                argumentsExpressions[i] = parameterCast;
            }

            var newExpression = Expression.New(constructor, argumentsExpressions);
            var lambda = Expression.Lambda<ObjectActivator>(Expression.Convert(newExpression, typeof(object)), param);
            return lambda.Compile();
        }

        public ObjectActivator GenerateDefaultActivator(Type type)
        {
            var body = Expression.Convert(Expression.Default(type), typeof(object));
            var lambda = Expression.Lambda<ObjectActivator>(body, Expression.Parameter(typeof(object[])));
            return lambda.Compile();
        }
    }
}