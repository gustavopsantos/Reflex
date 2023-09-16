using System;
using System.Linq;
using System.Reflection;

namespace Reflex.Microsoft.Exceptions
{
    internal sealed class MethodInjectorException : Exception
    {
        public MethodInjectorException(object obj, MethodInfo method, Exception e) : base(BuildMessage(obj, method, e))
        {
        }

        private static string BuildMessage(object obj, MethodInfo method, Exception e)
        {
			ParameterInfo[] parameters = method.GetParameters();
			string methodDescription = $"'{obj.GetType().Name}::{method.Name}'";
			string parametersDescription = $"'{string.Join(", ", parameters.Select(p => p.ParameterType.Name))}'";
            return $"Could not inject method {methodDescription} with parameters {parametersDescription}\n\n{e}";
        }
    }
}