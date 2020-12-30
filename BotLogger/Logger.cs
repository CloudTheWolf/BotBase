using Microsoft.Extensions.Logging;

namespace BotLogger
{
    public class Logger
    {

        public static ILoggerFactory BotLoggerFactory;

        public static ILogger<Logger> ConsoleLogger;

        public Logger(ILogger<Logger> logger)
        {
            ConsoleLogger = logger;
        }

        public static void LogInfo(string message)
        {
            ConsoleLogger.LogInformation(message);
        }
    }
}
