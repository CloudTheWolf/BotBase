using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BotLogger;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace TwitchTv.Module.Libs
{
    class DatabaseActions
    {
        public static ILogger<Logger> DbLogger;
        private static BotData.MySql _mySql;

        internal DatabaseActions()
        {
            DbLogger = Ttv.Logger;
            _mySql = new BotData.MySql(DbLogger);
        }

        internal async Task<DataTable> GetStreamers()
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("type", "twitch")
                };
                return await _mySql.RunProcedure("streams_GetStreams", args);
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
                return new DataTable();
            }
        }

        internal void UpdateStream(string id, string timestamp)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("timestamp", timestamp),
                    new KeyValuePair<string, object>("id", id)
                };
                _mySql.RunProcedure("stream_updateTimeStamp", args);
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal async void DeleteStream(string user, string guild)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("userid", user),
                    new KeyValuePair<string, object>("guildId", guild),
                    new KeyValuePair<string, object>("type", "twitch")
                };
                await _mySql.RunProcedure("streams_DeleteStream", args);
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal int CanAddStream(string channelName, ulong guildId, ulong userid)
        {
            try
            {
                var args = new List<KeyValuePair<string,object>>
                {
                    new KeyValuePair<string, object>("channel", channelName),
                    new KeyValuePair<string, object>("guildId", guildId),
                    new KeyValuePair<string, object>("userid", userid),
                    new KeyValuePair<string, object>("type", "twitch")
                };

                var dt = _mySql.RunProcedure("streams_GetStreamForChannelInGuild", args).Result;
                return dt.Rows.Count;
            }
            catch (Exception e)
            {
                Ttv.Logger.LogCritical(e.Message);
                return 99;
            }
        }

        internal void AddStream(string channelName, ulong user, ulong guildid, int ping = 0)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("channel", channelName),
                    new KeyValuePair<string, object>("ping", ping),
                    new KeyValuePair<string, object>("discordId", user),
                    new KeyValuePair<string, object>("guildId", guildid),
                    new KeyValuePair<string, object>("type", "twitch")
                };

                _mySql.RunProcedure("streams_AddStream", args);


            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal void UpgradeStream(ulong user, ulong guildId, int ping)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("userId", user),
                    new KeyValuePair<string, object>("guildId", guildId),
                    new KeyValuePair<string, object>("ping", ping)
                };
                _mySql.RunProcedure("streams_setPing", args);
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal DataTable GetAllStreams(ulong guildId)
        {
            
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("guildId", guildId),
                    new KeyValuePair<string, object>("type", "twitch")
                };
                return _mySql.RunProcedure("streams_GetAllStreamsForGuild", args).Result;
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
                return new DataTable();
            }

            
        }

        internal DataTable GetLastStream(ulong guildId)
        {
            
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("guildId", guildId),
                    new KeyValuePair<string, object>("type", "twitch")
                };
                return _mySql.RunProcedure("streams_getLastAnnounce", args).Result;
                

            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
                return new DataTable();
            }
        }

        internal DataTable GetSettingsForGuild(ulong guildId, string setting)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("guildId", guildId),
                    new KeyValuePair<string, object>("module", "twitch"),
                    new KeyValuePair<string, object>("setting", setting)
                };
                return _mySql.RunProcedure("settings_getSetting", args).Result;
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
                return new DataTable();
            }
        }

        internal async Task SetConfigInDb(ulong guildId, string setting, int iValue = default, string sValue = "",
            ulong biValue = default)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("guildId", guildId),
                new KeyValuePair<string, object>("module", "twitch"),
                new KeyValuePair<string, object>("setting", setting)
            };

            args.Add(iValue == default
                ? new KeyValuePair<string, object>("iValue", DBNull.Value)
                : new KeyValuePair<string, object>("iValue", iValue));

            args.Add(sValue == string.Empty
                ? new KeyValuePair<string, object>("sValue", DBNull.Value)
                : new KeyValuePair<string, object>("sValue", sValue));

            args.Add(biValue == default
                ? new KeyValuePair<string, object>("biValue", DBNull.Value)
                : new KeyValuePair<string, object>("biValue", biValue));
            await _mySql.RunProcedure("settings_AddUpdate", args);
        }
    }
}
