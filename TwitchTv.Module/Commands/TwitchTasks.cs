﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using TwitchTv.Module.Libs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace TwitchTv.Module.Commands
{
    public class TwitchTasks
    {

        private static readonly Twitch twitch = new Twitch();
        public static bool bRunning = false;
        private static readonly DatabaseActions da = new DatabaseActions();
        public ManualResetEvent CycleManualResetEvent;

        public static async Task LogAction(string message, DiscordClient c)
        {
            DiscordChannel logChannel = await c.GetChannelAsync(TwitchOptions.LogChannelId);
            await logChannel.SendMessageAsync(message);
        }

        public static bool IsMemberStillHere(ulong memberId, DiscordClient c, DiscordGuild g)
        {
            try
            {
                var member = g.GetMemberAsync(memberId).Result;
                if (member == null) return false;
                return true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Not found: 404"))
                {
                    var getInfo = c.GetUserAsync(memberId).Result;
                    LogAction($"{getInfo.Username} `{memberId}` no longer resides in the server... removing.", c).ConfigureAwait(false);
                    da.DeleteStream(memberId.ToString(), g.Id.ToString());
                }
                else
                {
                    Ttv.Logger.LogError(e.Message);
                    LogAction($"Error Loading Member\n```{e}```", c).ConfigureAwait(false);
                }
                return false;
            }
        }

        public static ulong ParseMemberToId(string id)
        {
            var userId = id.Replace("<", "");
            userId = userId.Replace(">", "");
            userId = userId.Replace("@", "");
            userId = userId.Replace("!", "");

            ulong memberId = ulong.Parse(userId);
            return memberId;
        }

        internal static async Task StartScanChannels(ReadyEventArgs readyEventArgs)
        {
            Task scanner = new Task(() => ScanChannels(readyEventArgs.Client));
            scanner.Start();
        }

        internal static async Task ScanChannels(DiscordClient client)
        {
            while (bRunning)
            {
                Ttv.Logger.LogInformation("Start Cycle");
                try
                {
                    var streams = await da.GetStreamers();
                    foreach (DataRow stream in streams.Rows)
                    {
                        try
                        {
                            var discordId = ulong.Parse(stream["discordId"].ToString());
                            var guild = client.GetGuildAsync(ulong.Parse(stream["guildId"].ToString()),
                                false).Result;
                            if (!IsMemberStillHere(discordId, client,guild)) continue;
                            var streamId = twitch.GetChannelId(stream["name"].ToString());
                            if (!twitch.IsOnline(streamId)) continue;
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
                                var channel = GetChannelFromId(client, ulong.Parse(stream["channelId"].ToString()));
                                await channel.SendMessageAsync($"{message}", embed: embed);
                                await LogAction($"{stream["name"]} has been promoted for <@!{discordId}> [{discordId}]",
                                    client);
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
                            if(e.Message.Contains("One or more errors occurred. (Not found: 404)")) continue;
                            Ttv.Logger.LogCritical($"{e}");
                            await LogAction($"TwitchTV Module encountered an error while scanning channel {stream["name"]} \n {e.Message}\n```{e}```", client);
                            
                        }
                    }
                }
                catch (Exception e)
                {
                    Ttv.Logger.LogCritical($"{e}");
                    await LogAction($"TwitchTV Module encountered an error in task `ScanChannels() ` \n {e.Message}\n```{e}```", client);
                }

                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
        }

        private static DiscordChannel GetChannelFromId(DiscordClient client, ulong id)
        {
            return client.GetChannelAsync(id).Result;
        }
    }
}
