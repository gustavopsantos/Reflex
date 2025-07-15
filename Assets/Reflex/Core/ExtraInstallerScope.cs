using System;
using Reflex.Injectors;

namespace Reflex.Core
{
    public class ExtraInstallerScope : IDisposable
    {
        private readonly Action<ContainerBuilder> _extraInstaller;

        public ExtraInstallerScope(Action<ContainerBuilder> extraInstaller)
        {
            _extraInstaller = extraInstaller;
            UnityInjector.ExtraInstallers += _extraInstaller;
        }

        public void Dispose()
        {
            UnityInjector.ExtraInstallers -= _extraInstaller;
        }
    }
}