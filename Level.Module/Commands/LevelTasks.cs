using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Level.Module.Libs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Level.Module.Commands
{
    public class LevelTasks
    {
        public static async Task LevelsClient_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;
            Level.Logger.LogDebug($"Give EXP to {e.Author.Username} [{e.Author.Id}]");
            await DatabaseActions.GiveExp(e.Author.Id.ToString());
        }

        public static async Task LevelsClient_MessageDeleted(MessageDeleteEventArgs e)
        {
            if(!LevelOptions.RemoveExpOnMessageDelete) return;
            if (e.Message.Author.IsBot) return;
            await DatabaseActions.RevokeExp(e.Message.Author.Id.ToString());
        }

        public static async Task LevelsClient_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            if(e.User.IsBot) return;
            var oldChannel = e.Before;
            var newChannel = e.After;
            var now = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm");
            if (oldChannel == null && newChannel != null)
            {
                Level.Logger.LogDebug($"{e.User.Id} has joined VC {e.Channel.Name}[{e.Channel.Id}]");
                await DatabaseActions.SetUserInCall(e.User.Id.ToString(),now,true);
                return;
            }

            if (oldChannel != null && newChannel.Channel == null)
            {
                Level.Logger.LogDebug($"{e.User.Id} is no longer in a voice channel");
                var user  = await DatabaseActions.GetUserExp(e.User.Id,true);
                await DatabaseActions.SetUserInCall(e.User.Id.ToString(),now, false);
                var diff = Convert.ToInt32((DateTime.Parse(now) - DateTime.Parse(user.Rows[0]["voice_last_join"].ToString()!)).TotalMinutes);
                decimal expToAdd = diff * LevelOptions.ExpPerVoiceMin;
                await DatabaseActions.GiveExp(e.User.Id.ToString(),true,expToAdd);
                return;
            }

            Level.Logger.LogDebug($"{e.User.Id} is still in VC, doing nothing");
            return;

        }

        public static DiscordEmbed BuildEmbedTask(string title, string author, string author_icon, List<KeyValuePair<string, string>> args, string avatar = "")
        {
            var embed = new DiscordEmbedBuilder()
            {
                Title = title,
                Color = DiscordColor.Gold,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = avatar },
                Author = new DiscordEmbedBuilder.EmbedAuthor() { Name = author, IconUrl = author_icon }
            };

            embed.Thumbnail.Url = avatar;


            foreach (var (key,value) in args)
            {
                embed.AddField(key, value,true);
            }

            return embed.Build();
        }

        public static async Task LevelClient_BanUser(GuildBanAddEventArgs e)
        {
            if(!LevelOptions.PurgeExpOnBan) return;
            if(e.Member.IsBot) return;

            await DatabaseActions.ResetExp(e.Member.Id.ToString());
        }
    }
}