using DSharpPlus.Entities;

namespace TwitchTv.Module.Libs
{
    class DiscordEmbed
    {
        public static DiscordEmbedBuilder TwitchEmbedBuilder(string title, string channel, string game, string views,
            string avatar, string preview,string followers, string level)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{channel} Is Live On Twitch!",
                Color = DiscordColor.Purple,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = avatar },
                Url = $"https://twitch.tv/{channel}"
            };
            embed.AddField("Title", title);
            embed.AddField("Channel", channel, true);
            embed.AddField("Game", game, true);
            embed.AddField("Viewers", $"{views}", true);
            embed.AddField("Followers", $"{followers}", true);
            embed.AddField("Level", $"{level}!", true);
            embed.ImageUrl = preview;
            return embed;
        }


        public static DiscordEmbedBuilder BotStatusBuilder(string lastSteam, string status, string channels, 
            string avatar, string features, bool large, string members, string splash)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"TwitchTV Module Status!",
                Color = DiscordColor.Gold,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = avatar }
            };
            embed.AddField("Total Streams", channels.ToString(), true);
            embed.AddField("Last Stream", lastSteam, true);
            embed.AddField("Large Guild", $"{large}", true);
            embed.AddField("Members", $"{members}", true);
            embed.AddField("Bot Running", $"{status}", true);
            embed.AddField("Features", $"{features}", true);
            embed.ImageUrl = splash;
            return embed;
        }
    }
}
