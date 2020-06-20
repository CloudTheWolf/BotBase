using System;
using System.Threading;
using System.Threading.Tasks;
using BotCore;
using BotLogger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace BotBase
{
    public class Worker : BackgroundService
    {
        public static ILogger<Logger> Logger;

        public Worker(ILogger<Logger> logger)
        {
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var bot = new Bot();
                await bot.RunAsync(stoppingToken, Logger);
                await Task.Delay(-1, stoppingToken);
				
            }
        }
    }
}
