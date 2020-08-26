using System;
using System.Threading;
using System.Threading.Tasks;
using BotLogger;
using BotShared.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Level.Module.Commands;
using Microsoft.Extensions.Logging;
using static Level.Module.Commands.LevelCommands;

namespace Level.Module
{
    public class Level : IPlugin
    {
        public string Name => "Simple Levels";
        public string Description => "A Simple Level System... Init!";
        public int Version => 2;

        public static ILogger<Logger> Logger;
        public static IBot Bot;
        public static InteractivityExtension Interactivity;
        private static DiscordConfiguration _discordConfiguration;
        private dynamic _myConfig;
        public static CancellationToken LevelCancellationToken;
        
        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, dynamic applicationConfig)
        {
            Logger = logger;
            _myConfig = applicationConfig;
            _discordConfiguration = discordConfiguration;
            SetConfig();
            logger.LogInformation($"{Name}: Loaded successfully!");
            bot.Commands.RegisterCommands<LevelCommands>();
            LevelCancellationToken = new CancellationToken(false);
            bot.Client.MessageCreated += Client_MessageCreated;
            bot.Client.MessageDeleted += Client_MessageDeleted;
            
            logger.LogInformation($"{Name}: Loaded Client_MessageCreated!");


        }

        

        private void SetConfig()
        {
            try
            {
                LevelOptions.ExpPerMessage = int.Parse(_myConfig.Level["MsgExp"].ToString());
                LevelOptions.ExpPerVoiceMin = int.Parse(_myConfig.Level["VoiceExp"].ToString());
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Error Setting EXP \n {e}");
                LevelOptions.ExpPerMessage = 5;
                LevelOptions.ExpPerVoiceMin = 10;
            }
        }


    }
}
