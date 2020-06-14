using DSharpPlus;

namespace BotShared.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        int Version { get; }
        void Start(DiscordConfiguration discordConfiguration, dynamic applicationConfig);
    }
}