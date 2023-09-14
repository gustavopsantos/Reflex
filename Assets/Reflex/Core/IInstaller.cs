using Microsoft.Extensions.DependencyInjection;

namespace Reflex.Core
{
    public interface IInstaller
    {
        void InstallBindings(IServiceCollection descriptor);
    }
}