using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;


namespace Example.Module.Commands
{
    internal class ExampleCommands : BaseCommandModule
    {
        [Command("ding")]
        public async Task Ding(CommandContext ctx)
        {
            Example.Logger.LogInformation("Ding Request From {name} ({id})", ctx.Member.Nickname, ctx.Member.Id);
            await ctx.Channel.SendMessageAsync("Dong").ConfigureAwait(false);
        }
    }
}
