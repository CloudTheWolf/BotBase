using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;


namespace Example.Module.Commands
{
    internal class ExampleCommands : BaseCommandModule
    {

        [Command("ping")]
        [RequireRoles(RoleCheckMode.Any, "Followers")]
        public async Task Ping(CommandContext ctx)
        {
            Example.Logger.LogInformation("Ping Request From {name} ({id})", ctx.Member.Nickname, ctx.Member.Id);
            await ctx.Channel.SendMessageAsync("Ping").ConfigureAwait(false);
        }

        [Command("debug.roles")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task GetAllRoles(CommandContext ctx)
        {
            Example.Logger.LogInformation("{name} ({id}) has requested a dump of all roles. Poggers!", ctx.Member.Nickname, ctx.Member.Id);
            await ctx.Message.DeleteAsync();
            var roles = ctx.Guild.Roles.ToList();
            var roleList = string.Empty;
            foreach (var role in roles)
            {
                roleList += role.Value + "\n";
            }

            File.WriteAllText("Roles.txt", roleList);
            using (var fs = new FileStream("Roles.txt", FileMode.Open, FileAccess.Read))
            {
                var message = new DiscordMessageBuilder().WithContent("Roles Attached").WithFiles(new Dictionary<string, Stream>() { { "Roles.txt", fs } });
                await ctx.Member.SendMessageAsync(message);
            }            
        }
    }
}
