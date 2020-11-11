using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using DSharpPlus;
using TwitchTv.Module.Libs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DiscordEmbed = TwitchTv.Module.Libs.DiscordEmbed;


namespace TwitchTv.Module.Commands
{
    internal class TwitchCommands : BaseCommandModule
    {
        private readonly Twitch twitch = new Twitch();
        private static readonly DatabaseActions da = new DatabaseActions();

        [Command("getChannels")]
        [Description("Get All Streams From DB")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task GetAllStreams(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            var data = da.GetAllStreams(ctx.Guild.Id);
            File.WriteAllText("Members.json", JsonConvert.SerializeObject(JArray.FromObject(data)));

            await ctx.Member.SendFileAsync("Members.json", "Roles Attached");
        }

        [Command("bot.status")]
        [Description("Manually Announce A Stream (No Ping)")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task Status(CommandContext ctx)
        {

            dynamic lastStream = JArray.FromObject(da.GetLastStream(ctx.Guild.Id));
            var streams = JArray.FromObject(da.GetAllStreams(ctx.Guild.Id));
            var avatar = ctx.Guild.IconUrl;
            var features = ctx.Guild.Features.Cast<string>();
            var large = ctx.Guild.IsLarge;
            var members = ctx.Guild.MemberCount;
            var splash = ctx.Guild.SplashUrl;
            var list = "";

            foreach (var feature in features)
            {
                list += feature.ToLower().Replace("_", " ") + ", ";
            }

            var embed = DiscordEmbed.BotStatusBuilder($"{lastStream[0]["Twitch"]} - <@!{lastStream[0]["discordId"]}>",
                $"{TwitchTasks.bRunning}", streams.Count().ToString(), avatar, list, large, members.ToString(),splash);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("ttv.status")]
        [Description("Manually Announce A Stream (No Ping)")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task TtvStatus(CommandContext ctx, [Description("Specify the channel you want to check")] string channel)
        {
            Ttv.Logger.LogInformation($"Do a thing");
            var streamId = twitch.GetChannelId(channel);
            if (!twitch.IsOnline(streamId)) return;
            var embed = twitch.BuildPromoEmbed(channel, true);
            await ctx.Channel.SendMessageAsync(embed: embed);
        }


        [Command("ttv.approve")]
        [Description("Approve a stream for @here mentions")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task Approve(CommandContext ctx, [Description("Specify the member you want to add")]
            string member)
        {
            var memberId = TwitchTasks.ParseMemberToId(member);
            da.UpgradeStream(memberId,ctx.Guild.Id,0);
            await ctx.RespondAsync($"Ok, I'll now `@here` mention for <@!{memberId}>'s Streams");
            await TwitchTasks.LogAction($"<@!{ctx.Member.Id}> has approved <@!{memberId}>'s Streams for `@here` mentions", ctx.Client);
            if(!TwitchOptions.AutoAssignRoles) return;
            var verifiedRoleIdRequest = da.GetSettingsForGuild(ctx.Guild.Id, "VerifiedRole");
            if(verifiedRoleIdRequest.Rows.Count == 0) return;
            if(string.IsNullOrWhiteSpace(verifiedRoleIdRequest.Rows[0]["biValue"].ToString())) return;
            var verifiedRoleId = ulong.Parse(verifiedRoleIdRequest.Rows[0]["biValue"].ToString()!);
            var verifiedStreamerRole = ctx.Guild.GetRole(verifiedRoleId);
            var discordMember = await ctx.Guild.GetMemberAsync(memberId);
            await discordMember.GrantRoleAsync(verifiedStreamerRole);
        }

        [Command("ttv.add")]
        [Description("Add a user to the Stream Auto-Announcement")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task AddStream(CommandContext ctx, [Description("Specify the Twitch channel you want to add")] string channel, 
            [Description("Mention the user to add")] string mention, [Description("Ping Here")] int ping)
        {
            
            var memberId = TwitchTasks.ParseMemberToId(mention);
            await ctx.Message.DeleteAsync();
            if (da.CanAddStream(channel,ctx.Guild.Id, ctx.Member.Id) > 0)
            {
                await ctx.RespondAsync("Failed To Add Stream, Member or Stream Already Added");
                return;
            }
            da.AddStream(channel, memberId,ctx.Guild.Id, ping);
            await ctx.RespondAsync($"{channel} has been added for <@!{memberId}>!");
            await TwitchTasks.LogAction($"<@!{ctx.Member.Id}> has enabled Stream Announcements for user <@!{memberId}> and channel https://twitch.tv/" 
                            + $"{channel}", ctx.Client);
            if(!TwitchOptions.AutoAssignRoles) return;
            try
            {
                var member = await ctx.Guild.GetMemberAsync(memberId);


                var streamerRole = GetRoleFromId(ctx.Guild.Id, "StreamerRole", ctx);

                await member.GrantRoleAsync(streamerRole);

                if (ping != 1) return;

                var verifiedStreamerRole = GetRoleFromId(ctx.Guild.Id, "VerifiedRole",ctx);
                await member.GrantRoleAsync(verifiedStreamerRole);

            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"Error Assigning Roles {e.Message}");
            }

        }

        private static DiscordRole GetRoleFromId(ulong guildId, string role, CommandContext ctx)
        {
            var streamerRoleIdRequest = da.GetSettingsForGuild(guildId, role);
            if (streamerRoleIdRequest.Rows.Count == 0 ||
                string.IsNullOrWhiteSpace(streamerRoleIdRequest.Rows[0]["biValue"].ToString())) throw new Exception("Role not Set");

            var streamerRoleId = ulong.Parse(streamerRoleIdRequest.Rows[0]["biValue"].ToString()!);
            var discordRole = ctx.Guild.GetRole(streamerRoleId);
            return discordRole;
        }

        [Command("stream.add")]
        [Description("Add your Twitch TV to the Auto Announcements")]
        public async Task AddStreamUser(CommandContext ctx, [Description("Your Channel Name")] string channel)
        {
            var memberId = ctx.Member.Id;
            await ctx.Message.DeleteAsync();
            if (da.CanAddStream(channel,ctx.Guild.Id,ctx.Member.Id) > 0)
            {
                await ctx.RespondAsync("Failed To Add Stream, Member or Stream Already Added");
                return;
            }
            da.AddStream(Uri.EscapeUriString(channel), memberId,ctx.Guild.Id);
            await ctx.RespondAsync($"<@!{memberId}>, I will add you to the Stream Notifications Channel");
            await TwitchTasks.LogAction($"<@!{memberId}> has enabled Stream Announcements for https://twitch.tv/" + $"{channel}", ctx.Client);
            if (!TwitchOptions.AutoAssignRoles) return;

            var streamerRole = GetRoleFromId(ctx.Guild.Id, "StreamerRole", ctx);

            await ctx.Member.GrantRoleAsync(streamerRole);
            

        }

        [Command("stream.remove")]
        [Description("Remove your Twitch TV from the Auto Announcements")]
        public async Task RemoveStreamUser(CommandContext ctx)
        {
            var memberId = ctx.Member.Id;
            await ctx.Message.DeleteAsync();
            da.DeleteStream(memberId.ToString(), ctx.Guild.Id.ToString());
            await ctx.RespondAsync($"<@!{memberId}>, I will stop announcing your stream in Stream Notifications Channel");
            await TwitchTasks.LogAction($"<@!{memberId}> has removed Stream Announcements", ctx.Client);
            if (!TwitchOptions.AutoAssignRoles) return;

            var member = await ctx.Guild.GetMemberAsync(memberId);
            var StreamerRole = GetRoleFromId(ctx.Guild.Id, "StreamerRole", ctx);
            var VerifiedStreamerRole = GetRoleFromId(ctx.Guild.Id, "VerifiedRole", ctx);
            await member.RevokeRoleAsync(VerifiedStreamerRole);
            await member.RevokeRoleAsync(StreamerRole);
            await TwitchTasks.LogAction($"Roles removed stream notifications for <@!{memberId}>", ctx.Client);

        }


        [Command("ttv.remove")]
        [Description("Remove a user from the Stream Auto-Announcement")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task RemoveStream(CommandContext ctx, [Description("Mention the user to remove")]  string mention)
        {
            var memberId = TwitchTasks.ParseMemberToId(mention);
            await ctx.Message.DeleteAsync();
            da.DeleteStream(memberId.ToString(),ctx.Guild.Id.ToString());
            await ctx.RespondAsync($"Stream Removed For <@!{memberId}>");
            await TwitchTasks.LogAction($"<@!{ctx.Member.Id}> has removed stream notifications for <@!{memberId}>", ctx.Client);
            if(!TwitchOptions.AutoAssignRoles) return;

            var member = await ctx.Guild.GetMemberAsync(memberId);
            var StreamerRole = GetRoleFromId(ctx.Guild.Id, "StreamerRole", ctx);
            var VerifiedStreamerRole = GetRoleFromId(ctx.Guild.Id, "VerifiedRole", ctx);
            await member.RevokeRoleAsync(VerifiedStreamerRole);
            await member.RevokeRoleAsync(StreamerRole);
            await TwitchTasks.LogAction($"Roles removed stream notifications for <@!{memberId}>", ctx.Client);

        }

        [Command("ttv.setup")]
        [Description("Configure Twitch Settings")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task SetupTask(CommandContext ctx, string setting, string value)
        {
            switch (setting)
            {
                default:
                    await ctx.RespondAsync("Invalid Setting Supplied");
                    break;
                case "StreamChannel":
                case "VerifiedRole":
                case "StreamerRole":
                case "UlTest":
                    await da.SetConfigInDb(ctx.Guild.Id, setting, biValue: ulong.Parse(value));
                    await ctx.RespondAsync("Request Sent");
                    break;
                
            }
        }



    }
}
