using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BotData;
using BotLogger;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Level.Module.Libs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;


namespace Level.Module.Commands
{
    public class LevelCommands : BaseCommandModule
    {
        public static bool bRunning;


        [Command("level")]
        public async Task GetUserLevel(CommandContext ctx)
        {
            var user = DatabaseActions.GetUserExp(ctx.User.Id).Result;
            if(user == null) return;
            var args = new List<KeyValuePair<string,string>>()
            {
                new KeyValuePair<string, string>("Level",user.Rows[0]["level"].ToString()),
                new KeyValuePair<string, string>("Exp",user.Rows[0]["exp"].ToString())
            };
            var avatar = ctx.Member.AvatarUrl;
            if (user.Rows[0]["badgeImageUrl"] != DBNull.Value) avatar = user.Rows[0]["badgeImageUrl"].ToString();
            DiscordEmbed embed = LevelTasks.BuildEmbedTask("Chat Exp!", "Levels!", ctx.Guild.IconUrl ,args,avatar);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("vc_level")]
        public async Task GetUserVoiceLevel(CommandContext ctx)
        {
            var user = DatabaseActions.GetUserExp(ctx.User.Id,true).Result;
            if (user == null) return;
            var args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Level",user.Rows[0]["level"].ToString()),
                new KeyValuePair<string, string>("Exp",user.Rows[0]["voice_exp"].ToString())
            };
            var avatar = ctx.Member.AvatarUrl;
            if (user.Rows[0]["badgeImageUrl"] != DBNull.Value) avatar = user.Rows[0]["badgeImageUrl"].ToString();
            DiscordEmbed embed = LevelTasks.BuildEmbedTask("Voice Exp!", "Levels!", ctx.Guild.IconUrl, args, avatar);
            await ctx.RespondAsync(embed: embed);            
        }

        
    }
}
