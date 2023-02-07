using System;
using System.Linq;
using System.Reflection;

namespace Reflex.Scripts.Caching
{
    internal sealed class InjectedMethodInfo
    {
        public readonly Type[] Parameters;

        private readonly MethodInfo _methodInfo;
        
        public InjectedMethodInfo(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            Parameters = methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
        }

        public void Invoke(object obj, object[] parameters)
        {
            _methodInfo.Invoke(obj, parameters);
        }
        
        public static implicit operator MethodInfo(InjectedMethodInfo injectedMethodInfo)
        {
            return injectedMethodInfo._methodInfo;
        }
    }
}