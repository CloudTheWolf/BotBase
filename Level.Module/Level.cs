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

        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration,
            dynamic applicationConfig)
        {
            Logger = logger;
            _myConfig = applicationConfig;
            _discordConfiguration = discordConfiguration;
            SetConfig();
            logger.LogInformation($"{Name}: Loaded successfully!");
            bot.Commands.RegisterCommands<LevelCommands>();
            LevelCancellationToken = new CancellationToken(false);
            bot.Client.MessageCreated += LevelTasks.LevelsClient_MessageCreated;
            bot.Client.MessageDeleted += LevelTasks.LevelsClient_MessageDeleted;
            bot.Client.VoiceStateUpdated += LevelTasks.LevelsClient_VoiceStateUpdated;
            bot.Client.GuildBanAdded += LevelTasks.LevelClient_BanUser;
            logger.LogInformation($"{Name}: Loaded LevelsClient_MessageCreated!");


        }

        private void SetConfig()
        {
            try
            {
                LevelOptions.ExpPerMessage = decimal.Parse(_myConfig.Level["MsgExp"].ToString());
                LevelOptions.ExpPerVoiceMin = decimal.Parse(_myConfig.Level["VoiceExp"].ToString());
                LevelOptions.PurgeExpOnBan = bool.Parse(_myConfig.Level["PurgeExpOnBan"].ToString());
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
