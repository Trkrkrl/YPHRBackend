using System;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities.IoC
{
    /// <summary>
    /// ServiceCollection'dan bir ServiceProvider üretip
    /// aspect'ler gibi statik yerlerden IoC container'a erişimi sağlar.
    /// </summary>
    public static class ServiceTool
    {
        /// <summary>
        /// Uygulama genelinde kullanılacak ServiceProvider referansı.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        /// Dışarıdan gelen IServiceCollection ile ServiceProvider üretir
        /// ve statik olarak saklar. Ardından aynı IServiceCollection'ı geri döner.
        /// </summary>
        public static IServiceCollection Create(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            return services;
        }
    }
}
