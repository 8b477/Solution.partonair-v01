using BLL.partonair_v01.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.partonair_v01.Services
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public IDisposable BeginScope(string messageFormat, params object[] args)
        {
            return _logger.BeginScope(messageFormat, args);
        }
    }
}
