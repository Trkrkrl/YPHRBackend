namespace Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels
{
    public class FileLogConfiguration
    {
        /// <summary>
        /// Folder path relative to application root. Example: "logs" or "Logs".
        /// </summary>
        public string FolderPath { get; set; } = "Logs";
    }
}
