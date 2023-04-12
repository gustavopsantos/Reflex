using System;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace Reflex.Weaving.Extensions
{
    public static class TypeExtensions
    {
        public static TypeDefinition AsDefinition(this Type type)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var module = assemblies.Single(a => a.FullName == type.Module.Assembly.FullName);
            using var stream = new FileStream(module.Location, FileMode.Open, FileAccess.Read);
            var moduleDefinition = ModuleDefinition.ReadModule(stream);
            return moduleDefinition.GetTypes().Single(t => t.FullName == type.FullName);
        }
    }
}