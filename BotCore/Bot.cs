using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
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
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Emzi0767.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BotCore
{
    public class Bot : IBot
    {
        public DiscordClient Client { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public DiscordRestClient Rest { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public InteractivityExtension Interactivity { get; set; }
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
            ListAllCommands();
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
            Options.Prefix = new string[] {_myConfig["prefix"].ToString()};
            Options.EnableDms = bool.Parse(_myConfig["enableDms"].ToString());
            Options.EnableMentionPrefix = bool.Parse(_myConfig["enableMentionPrefix"].ToString());
            Options.DmHelp = bool.Parse(_myConfig["dmHelp"].ToString());
            Options.DefaultHelp = bool.Parse(_myConfig["enableDefaultHelp"].ToString());

        }


        private void CreateClientCommandConfiguration()
        {
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = Options.Prefix,
                EnableDms = Options.EnableDms,
                EnableMentionPrefix = Options.EnableMentionPrefix,
                DmHelp = Options.DmHelp,
                EnableDefaultHelp = Options.DefaultHelp
            };
            
            Commands = Client.UseCommandsNext(commandsConfig);
        }

        private void CreateDiscordClient()
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
                LoggerFactory = BotLogger.Logger.BotLoggerFactory,
                
            };
        }

        private void ListAllCommands()
        {
            Console.WriteLine("Getting Commands");
            Console.WriteLine("============");
            foreach (var registeredCommand in Client.GetCommandsNext().RegisteredCommands)
            {
                Console.WriteLine($"Command: {registeredCommand.Value.Name} \nDesc: {registeredCommand.Value.Description} \nRoles:");
                foreach (var keyValuePair in registeredCommand.Value.Overloads.ToImmutableArray())
                {
                    foreach (var args in keyValuePair.Arguments)
                    {
                        Console.WriteLine($"> Name: {args.Name}\n> Desc: {args.Description}");
                    }
                }
                Console.WriteLine("============");
            }
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs readyEventArgs)
        {
            Logger.LogInformation($"Bot Ready With Prefix [{_myConfig["prefix"]}]");
            return Task.CompletedTask;
        }
    }
}