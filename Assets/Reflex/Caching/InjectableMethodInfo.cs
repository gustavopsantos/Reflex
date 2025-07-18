using System.Reflection;

namespace Reflex.Caching
{
    internal sealed class InjectableMethodInfo
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInfo[] Parameters;

        public InjectableMethodInfo(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
        }
    }
}