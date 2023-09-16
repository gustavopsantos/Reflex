using System;
using System.Linq.Expressions;
using System.Reflection;
using Reflex.Microsoft.Delegates;

namespace Reflex.Microsoft.Reflectors
{
	public sealed class MonoActivatorFactory : IActivatorFactory
	{
		public ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters)
		{
			ParameterExpression param = Expression.Parameter(typeof(object[]));
			Expression[] argumentsExpressions = new Expression[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ConstantExpression index = Expression.Constant(i);
				Type parameterType = parameters[i];
				BinaryExpression parameterAccessor = Expression.ArrayIndex(param, index);
				UnaryExpression parameterCast = Expression.Convert(parameterAccessor, parameterType);
				argumentsExpressions[i] = parameterCast;
			}

			NewExpression newExpression = Expression.New(constructor, argumentsExpressions);
			LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), Expression.Convert(newExpression, typeof(object)), param);
			return (ObjectActivator)lambda.Compile();
		}

		public ObjectActivator GenerateDefaultActivator(Type type)
		{
			UnaryExpression body = Expression.Convert(Expression.Default(type), typeof(object));
			LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), body, Expression.Parameter(typeof(object[])));
			return (ObjectActivator)lambda.Compile();
		}
	}
}