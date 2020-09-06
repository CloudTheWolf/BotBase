using System;
using System.Threading.Tasks;
using BotLogger;
using BotShared.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Logging;
using TwitchTv.Module.Commands;

namespace TwitchTv.Module
{
    public class Ttv : IPlugin
    {
        public string Name => "Twitch TV Notifications";
        public string Description => "A Twitch TV Plugin to notify when a member goes live";
        public int Version => 2;
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
                SetConfig();
                _discordConfiguration = discordConfiguration;
                logger.LogInformation($"{Name}: Loaded successfully!");
                bot.Commands.RegisterCommands<TwitchCommands>();
                Interactivity = bot.Interactivity;
                logger.LogInformation($"{Name}: Registered {nameof(TwitchCommands)}!");
                StartTwitchListener(bot);
                logger.LogInformation("TwitchListener Set");

            }
            catch (Exception e)
            {
                logger.LogCritical($"Failed to load {Name} \n {e}");
            }

        }


        private void SetConfig()
        {
            TwitchOptions.ClientId = _myConfig.twitch["ClientId"].ToString();
            TwitchOptions.AccessToken = _myConfig.twitch["AccessToken"].ToString();
            TwitchOptions.AutoAssignRoles = bool.Parse(_myConfig.twitch["AutoAssign"].ToString());
            TwitchOptions.AutoPurgeStreams = bool.Parse(_myConfig.twitch["AutoPurge"].ToString());
            TwitchOptions.StreamerRole = ulong.Parse(_myConfig.twitch["StreamerRole"].ToString());
            //TwitchOptions.VerifiedRole = ulong.Parse(_myConfig.twitch["VerifiedRole"].ToString());
            //TwitchOptions.TargetChannelId = ulong.Parse(_myConfig.twitch["StreamChannel"].ToString());
            TwitchOptions.LogChannelId = ulong.Parse(_myConfig.twitch["LogChannel"].ToString());
        }

        private void StartTwitchListener(IBot bot)
        {
            if(!bool.Parse(_myConfig.twitch["autostart"].ToString())) return;
            Logger.LogInformation("Auto-Start Twitch Channel Scan Enabled");
            TwitchTasks.bRunning = true;
            bot.Client.Ready += TwitchTasks.StartScanChannels;
        }
    }
}
