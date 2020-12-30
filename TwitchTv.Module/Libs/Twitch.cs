using System;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using DiscordEmbed = TwitchTv.Module.Libs.DiscordEmbed;

namespace TwitchTv.Module.Libs
{
    internal class Twitch
    {
        private static TwitchAPI api;


        internal Twitch()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = TwitchOptions.ClientId;
            api.Settings.AccessToken = TwitchOptions.AccessToken;

        }

        public bool IsOnline(string channelId)
        {
            var uptime = api.V5.Streams.GetUptimeAsync(channelId).Result;
            var channel = api.V5.Channels.GetChannelByIDAsync(channelId).Result;
            if (uptime != null) return true;
            Ttv.Logger.LogInformation($"{channel.DisplayName} is offline");
            return false;
        }

        public DiscordEmbedBuilder BuildPromoEmbed(string channelName, bool manual = false)
        {

            Ttv.Logger.LogInformation($"Find {channelName}");
            var channelId = GetChannelId(channelName);
            Ttv.Logger.LogInformation($"ID is {channelId}");
            var durationOnline = api.V5.Streams.GetUptimeAsync(channelId).Result.Value;
            if ((durationOnline.TotalMinutes < 1 || durationOnline.TotalMinutes > 10) && !manual) return null;
            var stream = api.V5.Streams.GetStreamByUserAsync(channelId).Result;
            var user = stream.Stream.Channel.DisplayName;
            var avatar = stream.Stream.Channel.Logo;
            var title = stream.Stream.Channel.Status;
            var preview = stream.Stream.Preview.Large;
            var game = stream.Stream.Game;
            var viewers = stream.Stream.Viewers.ToString();
            var followers = stream.Stream.Channel.Followers.ToString();
            var channel = api.V5.Channels.GetChannelByIDAsync(channelId).Result;
            var level = channel.BroadcasterType;
            switch (level)
            {
                case "":
                    level = "Road To Affiliate";
                    break;
                case "affiliate":
                    level = "Affiliate";
                    break;
                case "partner":
                    level = "Partner";
                    break;
            }
            if (string.IsNullOrWhiteSpace(level)) level = "Road To Affiliate";
            Ttv.Logger.LogInformation($"{title} + {user} + {preview} + {followers} + {level}");
            return DiscordEmbed.TwitchEmbedBuilder(title, user, game, viewers, avatar, preview, followers, level);
        }

        public string GetChannelId(string channelName)
        {
            try
            {
                var channels = api.V5.Users.GetUserByNameAsync(channelName);
                return channels.Result.Matches[0].Id;
            }
            catch (TwitchLib.Api.Core.Exceptions.BadRequestException e)
            {
                Ttv.Logger.LogError($"Scan Error: {e}");
                return string.Empty;
            }
        }

        
    }
}
