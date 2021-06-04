using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Level.Module.Libs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Level.Module.Commands
{ 
    public class LevelTasks
    {
        public static async Task LevelsClient_MessageCreated(DiscordClient sender, MessageCreateEventArgs messageCreateEventArgs)
        {
            if (messageCreateEventArgs.Author.IsBot) return;
            Level.Logger.LogDebug($"Give EXP to {messageCreateEventArgs.Author.Username} [{messageCreateEventArgs.Author.Id}]");
            await DatabaseActions.GiveExp(messageCreateEventArgs.Author.Id.ToString());
        }

        public static async Task LevelsClient_MessageDeleted(DiscordClient sender, MessageDeleteEventArgs messageDeleteEventArgs)
        {
            if(!LevelOptions.RemoveExpOnMessageDelete) return;
            if (messageDeleteEventArgs.Message.Author.IsBot) return;
            await DatabaseActions.RevokeExp(messageDeleteEventArgs.Message.Author.Id.ToString());
        }

        public static async Task LevelsClient_VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs voiceStateUpdateEventArgs)
        {
            if(voiceStateUpdateEventArgs.User.IsBot) return;
            var oldChannel = voiceStateUpdateEventArgs.Before;
            var newChannel = voiceStateUpdateEventArgs.After;
            var now = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm");
            if (oldChannel == null && newChannel != null)
            {
                Level.Logger.LogDebug($"{voiceStateUpdateEventArgs.User.Id} has joined VC {voiceStateUpdateEventArgs.Channel.Name}[{voiceStateUpdateEventArgs.Channel.Id}]");
                await DatabaseActions.SetUserInCall(voiceStateUpdateEventArgs.User.Id.ToString(),now,true);
                return;
            }

            if (oldChannel != null && newChannel.Channel == null)
            {
                Level.Logger.LogDebug($"{voiceStateUpdateEventArgs.User.Id} is no longer in a voice channel");
                var user  = await DatabaseActions.GetUserExp(voiceStateUpdateEventArgs.User.Id,true);
                await DatabaseActions.SetUserInCall(voiceStateUpdateEventArgs.User.Id.ToString(),now, false);
                var diff = Convert.ToInt32((DateTime.Parse(now) - DateTime.Parse(user.Rows[0]["voice_last_join"].ToString()!)).TotalMinutes);
                decimal expToAdd = diff * LevelOptions.ExpPerVoiceMin;
                await DatabaseActions.GiveExp(voiceStateUpdateEventArgs.User.Id.ToString(),true,expToAdd);
                return;
            }

            Level.Logger.LogDebug($"{voiceStateUpdateEventArgs.User.Id} is still in VC, doing nothing");
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
    }
}