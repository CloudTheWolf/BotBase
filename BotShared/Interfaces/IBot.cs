using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
namespace BotShared.Interfaces
{
    public interface IBot
    {
        CommandsNextExtension Commands { get; set; }
        InteractivityExtension Interactivity { get; set; }
        
    }
}