using Microsoft.Extensions.Logging;

namespace Local.BotLogger
{
    public class Logger
    {

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
