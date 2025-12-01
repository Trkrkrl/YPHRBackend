using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyResolvers
{
    /// <summary>
    /// Defines a module that can register core-level services into the DI container.
    /// </summary>
    public interface ICoreModule
    {
        void Load(IServiceCollection services);
    }
}
