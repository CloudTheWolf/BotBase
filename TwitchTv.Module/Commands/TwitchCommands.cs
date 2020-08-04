using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BotLogger;
using TwitchTv.Module.Libs;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static TwitchTv.Module.Ttv;
using DiscordEmbed = TwitchTv.Module.Libs.DiscordEmbed;


namespace TwitchTv.Module.Commands
{
    internal class TwitchCommands : BaseCommandModule
    {
        private Twitch twitch = new Twitch();
        private bool bRunning = false;
        private DatabaseActions da;
        public ManualResetEvent CycleManualResetEvent;

        public TwitchCommands()
        {
            da = new DatabaseActions();
            
        }

        [Command("getChannels")]
        [Description("Get All Streams From DB")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task GetAllStreams(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            var data = da.GetAllStreams();
            File.WriteAllText("Members.json", data);

            await ctx.Member.SendFileAsync("Members.json", "Roles Attached");
        }

        [Command("bot.status")]
        [Description("Manually Announce A Stream (No Ping)")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task Status(CommandContext ctx)
        {

            dynamic lastStream = JArray.Parse(da.GetLastStream());
            var streams = JArray.Parse(da.GetAllStreams());
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
                $"{bRunning}", streams.Count().ToString(), avatar, list, large, members.ToString(),splash);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("ttv.status")]
        [Description("Manually Announce A Stream (No Ping)")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task TtvStatus(CommandContext ctx, [Description("Specify the channel you want to check")] string channel)
        {
            Ttv.Logger.LogInformation($"Do a thing");
            var streamId = twitch.GetChannelId(channel);
            if (!twitch.isOnline(streamId)) return;
            var embed = twitch.BuildPromoEmbed(channel, true);
            await ctx.Channel.SendMessageAsync(embed: embed);
        }

        [Command("ttv.start")]
        [Description("Start the Auto-Announcement System")]
        [RequireRoles(RoleCheckMode.Any, "Discord Mod")]
        public async Task TtvStart(CommandContext ctx)
        {
            if(!bRunning)
            {
                bRunning = true;
                await ctx.Channel.SendMessageAsync("Will start checking for streamers going live");
                #pragma warning disable CS4014 
                ScanChannels(ctx, ctx.Guild.GetChannel(TwitchOptions.TargetChannelId)).ConfigureAwait(false);
                #pragma warning restore CS4014 
                return;
            }
            await ctx.Channel.SendMessageAsync("I am already running...");

        }

        [Command("ttv.stop")]
        [Description("Stop the Auto-Announcement System")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task TtvStop(CommandContext ctx)
        {
            if (bRunning)
            {
                bRunning = false;
                await ctx.Channel.SendMessageAsync("Ok, I'll stop after this cycle");
                CycleManualResetEvent.Set();
                return;
            }
            await ctx.Channel.SendMessageAsync("I am not running...");

        }

        [Command("ttv.approve")]
        [Description("Approve a stream for @here mentions")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task Approve(CommandContext ctx, [Description("Specify the member you want to add")]
            string member)
        {
            var memberId = ParseMemberToId(member);
            da.UpgradeStream(memberId);
            await ctx.RespondAsync($"Ok, I'll now `@here` mention for <@!{memberId}>'s Streams");
            await LogAction($"<@!{ctx.Member.Id}> has approved <@!{memberId}>'s Streams for `@here` mentions", ctx);
            if(!TwitchOptions.AutoAssignRoles) return;
            var VerifiedStreamerRole = ctx.Guild.GetRole(TwitchOptions.VerifiedRole);
            var discordMember = await ctx.Guild.GetMemberAsync(memberId);
            await discordMember.GrantRoleAsync(VerifiedStreamerRole);
        }

        [Command("ttv.add")]
        [Description("Add a user to the Stream Auto-Announcement")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task AddStream(CommandContext ctx, [Description("Specify the Twitch channel you want to add")] string channel, 
            [Description("Mention the user to add")] string mention, [Description("Ping Here")] int ping)
        {
            
            var memberId = ParseMemberToId(mention);
            await ctx.Message.DeleteAsync();
            if (da.CanAddStream(memberId, channel) > 0)
            {
                await ctx.RespondAsync("Failed To Add Stream, Member or Stream Already Added");
                return;
            }
            da.AddStream(channel, memberId,ping);
            await ctx.RespondAsync($"{channel} has been added for <@!{memberId}>!");
            await LogAction($"<@!{ctx.Member.Id}> has enabled Stream Announcements for user <@!{memberId}> and channel https://twitch.tv/" 
                            + $"{channel}", ctx);
            if(!TwitchOptions.AutoAssignRoles) return;
            var member = await ctx.Guild.GetMemberAsync(memberId);
            var VerifiedStreamerRole = ctx.Guild.GetRole(TwitchOptions.VerifiedRole);
            var StreamerRole = ctx.Guild.GetRole(TwitchOptions.StreamerRole);
            await member.GrantRoleAsync(StreamerRole);
            if (ping == 1) await member.GrantRoleAsync(VerifiedStreamerRole);
            

        }

        [Command("stream.add")]
        [Description("Add your Twitch TV to the Auto Announcements")]
        public async Task AddStreamUser(CommandContext ctx, [Description("Your Channel Name")] string channel)
        {
            var memberId = ctx.Member.Id;
            await ctx.Message.DeleteAsync();
            if (da.CanAddStream(memberId, channel) > 0)
            {
                await ctx.RespondAsync("Failed To Add Stream, Member or Stream Already Added");
                return;
            }
            da.AddStream(Uri.EscapeUriString(channel), memberId);
            await ctx.RespondAsync($"<@!{memberId}>, I will add you to the <#713084294490488933>");
            await LogAction($"<@!{memberId}> has enabled Stream Announcements for https://twitch.tv/" + $"{channel}", ctx);
            if (!TwitchOptions.AutoAssignRoles) return;
            var StreamerRole = ctx.Guild.GetRole(TwitchOptions.StreamerRole);
            await ctx.Member.GrantRoleAsync(StreamerRole);
            

        }

        [Command("stream.remove")]
        [Description("Remove your Twitch TV from the Auto Announcements")]
        public async Task RemoveStreamUser(CommandContext ctx)
        {
            var memberId = ctx.Member.Id;
            await ctx.Message.DeleteAsync();
            da.DeleteStream(memberId.ToString());
            await ctx.RespondAsync($"<@!{memberId}>, I will stop announcing your stream in <#713084294490488933>");
            await LogAction($"<@!{memberId}> has removed Stream Announcements", ctx);
            if (!TwitchOptions.AutoAssignRoles) return;

            var member = await ctx.Guild.GetMemberAsync(memberId);
            var StreamerRole = ctx.Guild.GetRole(TwitchOptions.StreamerRole);
            var VerifiedStreamerRole = ctx.Guild.GetRole(TwitchOptions.VerifiedRole);
            await member.RevokeRoleAsync(VerifiedStreamerRole);
            await member.RevokeRoleAsync(StreamerRole);
            await LogAction($"Roles removed stream notifications for <@!{memberId}>", ctx);

        }


        [Command("ttv.remove")]
        [Description("Remove a user from the Stream Auto-Announcement")]
        [RequireRoles(RoleCheckMode.Any, "TTVMod")]
        public async Task RemoveStream(CommandContext ctx, [Description("Mention the user to remove")]  string mention)
        {
            var memberId = ParseMemberToId(mention);
            await ctx.Message.DeleteAsync();
            da.DeleteStream(memberId.ToString());
            await ctx.RespondAsync($"Stream Removed For <@!{memberId}>");
            await LogAction($"<@!{ctx.Member.Id}> has removed stream notifications for <@!{memberId}>", ctx);
            if(!TwitchOptions.AutoAssignRoles) return;

            var member = await ctx.Guild.GetMemberAsync(memberId);
            var StreamerRole = ctx.Guild.GetRole(TwitchOptions.StreamerRole);
            var VerifiedStreamerRole = ctx.Guild.GetRole(TwitchOptions.VerifiedRole);
            await member.RevokeRoleAsync(VerifiedStreamerRole);
            await member.RevokeRoleAsync(StreamerRole);
            await LogAction($"Roles removed stream notifications for <@!{memberId}>", ctx);

        }

        public async Task LogAction(string message, CommandContext ctx)
        {
            DiscordChannel logChannel = await ctx.Client.GetChannelAsync(TwitchOptions.LogChannelId);
            await logChannel.SendMessageAsync(message);
        }


        public bool IsMemberStillHere(ulong memberId, CommandContext ctx)
        {
            try
            {
                var member = ctx.Guild.GetMemberAsync(memberId).Result;
                if (member == null) return false;
                return true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Not found: 404"))
                {
                    var getInfo = ctx.Client.GetUserAsync(memberId).Result;
                    LogAction($"{getInfo.Username} `{memberId}` no longer resides in The Cove, Will stop sending to <#713084294490488933>", ctx).ConfigureAwait(false);
                    da.DeleteStream(memberId.ToString());
                }
                else
                {
                    Ttv.Logger.LogError(e.Message);
                    LogAction($"Error Loading Member\n```{e}```", ctx).ConfigureAwait(false);
                }
                return false;
            }
        }

        private static ulong ParseMemberToId(string id)
        {
            var userId = id.Replace("<", "");
            userId = userId.Replace(">", "");
            userId = userId.Replace("@", "");
            userId = userId.Replace("!", "");

            ulong memberId = ulong.Parse(userId);
            return memberId;
        }

        private async Task ScanChannels(CommandContext ctx, DiscordChannel channel)
        {
            while (bRunning)
            {
                Ttv.Logger.LogInformation("Start Cycle");
                try
                {
                    var streams = da.getStreamers();
                    foreach (DataRow stream in streams.Rows)
                    {
                        try
                        {
                            var discordId = ulong.Parse(stream["discordId"].ToString());
                            if (!IsMemberStillHere(discordId, ctx)) continue;
                            var streamId = twitch.GetChannelId(stream["name"].ToString());
                            if (!twitch.isOnline(streamId)) continue;
                            var message = string.Empty;
                            switch (stream["approved"].ToString())
                            {
                                case "1":
                                {
                                    message = $"🔴 Hey @here! <@!{discordId}> Is Live!";
                                    break;
                                }
                                default:
                                {
                                    message = $"🔴 Hey All! <@!{discordId}> Is Live!";
                                    break;
                                }
                            }

                            try
                            {
                                var embed = twitch.BuildPromoEmbed(stream["name"].ToString());
                                if (embed == null)
                                {
                                    throw new Exception("No Embed Created");
                                }

                                await channel.SendMessageAsync($"{message}", embed: embed);
                                await LogAction($"{stream["name"]} has been promoted for <@!{discordId}> [{discordId}]",
                                    ctx);
                            }
                            catch (Exception)
                            {
                                Ttv.Logger.LogInformation($"No Embed Created For {stream["name"]}, Will skip.");
                                continue;
                            }

                            var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
                            da.UpdateStream(stream["id"].ToString(), now);

                        }
                        catch (Exception e)
                        {
                            Ttv.Logger.LogCritical($"{e}");
                            await LogAction($"TwitchTV Module encountered an error while scanning channel {stream["name"]} \n {e.Message}\n```{e}```", ctx);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                   Ttv.Logger.LogCritical($"{e}");
                   await LogAction($"TwitchTV Module encountered an error in task `ScanChannels() ` \n {e.Message}\n```{e}```", ctx);
                }

                CycleManualResetEvent.WaitOne(TimeSpan.FromSeconds(30));
            }
        }
    }
}
