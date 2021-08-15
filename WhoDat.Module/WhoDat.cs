using BotLogger;
using BotShared.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Logging;
using System;
using WhoDat.Module.Commands;

namespace WhoDat.Module
{
    public class WhoDat : IPlugin 
    {
        public string Name => "Guess Who Clone";
        public string Description => "Guess Who Game";
        public int Version => 1;
        public static ILogger<Logger> Logger;
        public InteractivityExtension Interactivity;
        private static DiscordConfiguration _discordConfiguration;
        private dynamic _myConfig;


        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, dynamic applicationConfig)
        {
            Logger = logger;

            try
            {

                _myConfig = applicationConfig;               
                _discordConfiguration = discordConfiguration;
                logger.LogInformation($"{Name}: Loaded successfully!");               
                Interactivity = bot.Interactivity;
                bot.Commands.RegisterCommands<GameCommands>();
                logger.LogInformation($"{Name}: Registered {nameof(GameCommands)}!");               

            }
            catch (Exception e)
            {
                logger.LogCritical($"Failed to load {Name} \n {e}");
            }

        }
    }
}
