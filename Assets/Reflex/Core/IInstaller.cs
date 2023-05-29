using System;

namespace Reflex.Core
{
    public interface IInstaller
    {
        void InstallBindings(ContainerDescriptor descriptor);
        void OnContainerBuilt(Container container);
    }
}