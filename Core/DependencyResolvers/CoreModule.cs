using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;

namespace Core.DependencyResolvers
{
    /// <summary>
    /// Registers core infrastructure services (cache, http context, logging, etc.).
    /// </summary>
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection services)
        {
            // ---- Required for the homework ----

            // In-memory cache (used by MemoryCacheManager + CacheAspect)
            services.AddMemoryCache();

            // Access to HttpContext (used by LogAspect / ExceptionLogAspect to get current user name)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Our cache abstraction -> current implementation uses Microsoft MemoryCache
            services.AddSingleton<ICacheManager, MemoryCacheManager>();

            // Serilog file logger; aspects will resolve this via ServiceTool + typeof(FileLogger)
            services.AddSingleton<FileLogger>();

            // Optional: also expose it as LoggerServiceBase for direct DI if needed
            // services.AddSingleton<LoggerServiceBase, FileLogger>();

            // ---- NOT REQUIRED for the homework (kept as reference) ----
            // Below services are examples for a more advanced infrastructure.
            // They are commented out because corresponding interfaces/classes are not generated.

            // IEmailConfiguration: SMTP / mail server configuration (host, port, credentials, etc.)
            // services.AddSingleton<IEmailConfiguration, EmailConfiguration>();

            // IMessageBrokerHelper: abstraction over message broker (RabbitMQ, Kafka, etc.)
            // services.AddTransient<IMessageBrokerHelper, MqQueueHelper>();

            // IMailService: application-level service to send e-mails using IEmailConfiguration
            // services.AddTransient<IMailService, MailManager>();
        }
    }
}
