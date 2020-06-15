using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotCore.Services;
using BotLogger;
using BotShared.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using LogLevel = DSharpPlus.LogLevel;

namespace BotCore
{
    public class Bot : IBot
    {
        private bool loadBase = true;
        public static DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        private static readonly PluginLoader PluginLoader = new PluginLoader();

        public static ILogger<Logger> Logger;


        public async Task RunAsync(CancellationToken stoppingToken, ILogger<Logger> logger)
        {
            Logger = logger;
            Logger.LogInformation("Bot Starting!");
            await LoadConfig();
            SetDiscordConfig();
            CreateDiscordClient();
            CreateClientCommandConfiguration();
            InitPlugins();

            await Client.ConnectAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private void InitPlugins()
        {
            PluginLoader.LoadPlugins();

            foreach (var plugin in PluginLoader.Plugins)
            {
                plugin.InitPlugin(this, Logger, _config, _myConfig);
            }
        }

        private static async Task LoadConfig()
        {
            const string configPath = "config.json";

            string json;
            using (var fs = File.OpenRead(configPath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            {
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }

            _myConfig = JObject.Parse(json);
        }

        private void CreateClientCommandConfiguration()
        {
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {_myConfig["prefix"].ToString()},
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
        }

        private static void CreateDiscordClient()
        {
            Client = new DiscordClient(_config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
        }

        private static void SetDiscordConfig()
        {
            _config = new DiscordConfiguration
            {
                Token = _myConfig["token"].ToString(),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
        }

        private static Task OnClientReady(ReadyEventArgs e)
        {
            Logger.LogInformation($"Bot Ready With Prefix [{_myConfig["prefix"]}]");
            return Task.CompletedTask;
        }
    }
}