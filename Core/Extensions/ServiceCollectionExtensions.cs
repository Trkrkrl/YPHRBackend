using Microsoft.Extensions.DependencyInjection;
using Core.DependencyResolvers;
using Core.Utilities.IoC;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all given ICoreModule implementations and then
        /// creates the ServiceProvider via ServiceTool for global access.
        /// </summary>
        public static IServiceCollection AddDependencyResolvers(
            this IServiceCollection services,
            ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Load(services);
            }

            return ServiceTool.Create(services);
        }
    }
}
