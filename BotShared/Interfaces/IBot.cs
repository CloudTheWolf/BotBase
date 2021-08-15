using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.Lavalink;

namespace BotShared.Interfaces
{
    public interface IBot
    {
        CommandsNextExtension Commands { get; set; }
        InteractivityExtension Interactivity { get; set; }
        DiscordClient Client { get; set; }
        VoiceNextExtension Voice { get; set; }
        DiscordRestClient Rest { get; set; }
        LavalinkConfiguration LvalinkConfig { get; set; }
    }
}