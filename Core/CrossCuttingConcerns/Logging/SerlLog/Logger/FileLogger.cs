using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.CrossCuttingConcerns.Logging.Serilog.Messages;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Logger
{
    public class FileLogger : LoggerServiceBase
    {
        private readonly IConfiguration _configuration;

        public FileLogger(IConfiguration configuration)
        {
            _configuration = configuration;

            var logConfig = _configuration
                .GetSection("SerilogConfigurations:FileLogConfiguration")
                .Get<FileLogConfiguration>()
                ?? throw new Exception(SerilogMessages.NullOptionsMessage);

            var folderPath = string.IsNullOrWhiteSpace(logConfig.FolderPath)
                ? "Logs"
                : logConfig.FolderPath.Trim('/','\\');

            var basePath = Directory.GetCurrentDirectory();
            var fullFolderPath = Path.Combine(basePath, folderPath);
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            // log-2025-01-01.txt gibi günlük dosyalar
            var logFilePath = Path.Combine(fullFolderPath, "log-.txt");

            Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    fileSizeLimitBytes: 5_000_000,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
