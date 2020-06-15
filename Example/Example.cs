using BotShared.Interfaces;
using DSharpPlus;
using Example.Module.Commands;
using Microsoft.Extensions.Logging;
using BotLogger;


namespace Example.Module
{
    public class Example : IPlugin
    {
        public string Name => "Example Plugin";
        public string Description => "An example to test that the plugin loader is working!";
        public int Version => 1;

        public static ILogger<Logger> Logger;
        private static DiscordConfiguration _discordConfiguration;
        private dynamic _myConfig;

        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, dynamic applicationConfig)
        {
            Logger = logger;
            _myConfig = applicationConfig;
            _discordConfiguration = discordConfiguration;
            logger.LogInformation($"{Name}: Loaded successfully!");

            bot.Commands.RegisterCommands<ExampleCommands>();
            logger.LogInformation($"{Name}: Registered {nameof(ExampleCommands)}!");
        }
    }
}
