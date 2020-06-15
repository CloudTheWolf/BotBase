using BotLogger;
using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace BotShared.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        int Version { get; }
        void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, dynamic applicationConfig);
    }
}