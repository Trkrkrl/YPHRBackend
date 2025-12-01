using System.Collections.Generic;

namespace Core.CrossCuttingConcerns.Logging
{
    public class LogDetail
    {
        public string FullName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public List<LogParameter> Parameters { get; set; } = new();
    }
}
