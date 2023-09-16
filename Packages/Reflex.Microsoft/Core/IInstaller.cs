using Microsoft.Extensions.DependencyInjection;

namespace Reflex.Microsoft.Core
{
    public interface IInstaller
    {
        void InstallBindings(IServiceCollection descriptor);
    }
}