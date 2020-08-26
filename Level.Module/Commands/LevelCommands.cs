using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
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


        [Command("lt")]
        public async Task aaa(CommandContext ctx)
        {
            Level.Logger.LogCritical("Something Red");
        }

        public static async Task LevelsClient_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;
            await DatabaseActions.GiveExp(e.Author.Id.ToString());
        }

        public static async Task LevelsClient_MessageDeleted(MessageDeleteEventArgs e)
        {
            if (e.Message.Author.IsBot) return;
            //TODO: Revoke EXP
        }

        public static async Task LevelsClient_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            if(e.User.IsBot) return;
            //TODO: Add Voice State EXP
        }
    }
}
