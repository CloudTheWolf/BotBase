using BotCore;
using BotShared.Interfaces;
using DSharpPlus;
using Example.Module.Commands;


namespace Example.Module
{
    public class Example : IPlugin
    {
        public string Name => "Example Plugin";
        public string Description => "An example to test that the plugin loader is working!";
        public int Version => 1;

        private static DiscordConfiguration _discordConfiguration;
        private dynamic _myConfig;

        public void Start(DiscordConfiguration discordConfiguration, dynamic applicationConfig)
        {
            _myConfig = applicationConfig;
            _discordConfiguration = discordConfiguration;
           Bot.Commands.RegisterCommands<ExampleCommands>();
        }
    }

}
