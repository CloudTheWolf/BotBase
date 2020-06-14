using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotCore.CommandSet;
using Microsoft.Extensions.Logging;
using BotLogger;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json.Linq;

namespace BotCore
{
    public class Bot
    {
        private bool loadBase = true;
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        private static PluginLoader pluginLoader = new PluginLoader();

        public static ILogger<Logger> Logger;



        public static async Task RunAsync(CancellationToken stoppingToken, ILogger<Logger> logger)
        {
            Logger = logger;
            Logger.LogInformation("Bot Starting!");
            var json = String.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            _myConfig = JObject.Parse(json);
            SetDiscordConfig();
            CreateDiscordClient();

            CreateClientCommandConfiguration();
            await Client.ConnectAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private static void CreateClientCommandConfiguration()
        {
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {_myConfig["prefix"].ToString()},
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<BaseCommands>();
            foreach (KeyValuePair<string, Command> command in Commands.RegisteredCommands)
            {
                Console.WriteLine($"{command.Key} = {command.Value.Description}");
            }
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
                LogLevel = DSharpPlus.LogLevel.Debug,
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
