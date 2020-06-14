using System;
using System.Security.Cryptography.X509Certificates;
using BotCore;
using BotLogger;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Example.Module.Commands;


namespace Example.Module
{
    public class Example : IPlugin
    {

        private static DiscordConfiguration _discordConfiguration;
        private dynamic _myConfig;


        public string Name
        {
            get
            {
                return "Example";
            }
        }

        public string Explanation
        {
            get
            {
                return "An Example Plugin";
            }
        }


        public void Start(DiscordConfiguration discordConfiguration, dynamic applicationConfig)
        {
            _myConfig = applicationConfig;
            _discordConfiguration = discordConfiguration;
           Bot.Commands.RegisterCommands<ExampleCommands>();
        }
    }

}
