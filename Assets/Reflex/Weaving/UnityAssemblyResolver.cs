using System.IO;
using Mono.Cecil;

namespace Reflex.Weaving
{
    public class UnityAssemblyResolver : IAssemblyResolver
    {
        private readonly IAssemblyResolver _resolver;

        public UnityAssemblyResolver()
        {
            _resolver = CreateResolver();
        }

        public void Dispose()
        {
            _resolver.Dispose();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return _resolver.Resolve(name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return _resolver.Resolve(name, parameters);
        }

        private static DefaultAssemblyResolver CreateResolver()
        {
            var resolver = new DefaultAssemblyResolver();

            foreach (var dir in Directory.GetDirectories("Library", "*", SearchOption.AllDirectories))
            {
                resolver.AddSearchDirectory(dir);
            }

            return resolver;
        }
    }
}