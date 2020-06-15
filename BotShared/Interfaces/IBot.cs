using DSharpPlus.CommandsNext;

namespace BotShared.Interfaces
{
    public interface IBot
    {
        CommandsNextExtension Commands { get; set; }
    }
}