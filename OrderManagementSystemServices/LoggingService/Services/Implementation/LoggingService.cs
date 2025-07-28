using LoggingService.Services;
using Microsoft.Extensions.Logging;

namespace LoggingService.Services.Implementation
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        public LoggingService(ILogger<LoggingService> logger)
        { 
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation("CONSUMER: {Message}", message);
        }
    }
} 