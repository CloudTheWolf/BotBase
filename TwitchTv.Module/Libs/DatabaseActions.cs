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
                    new KeyValuePair<string, object>("_type", "twitch")
                };
                return await _mySql.RunProcedure("streams_GetStreams", args);
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"SQL Error:{e.Message}\n{e}");
                return new DataTable();
            }
        }

        internal void UpdateStream(string id, string timestamp)
        {
            try
            {
                var args = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("_timestamp", timestamp),
                    new KeyValuePair<string, object>("_id", id)
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
                    new KeyValuePair<string, object>("_userid", user),
                    new KeyValuePair<string, object>("_guildId", guild),
                    new KeyValuePair<string, object>("_type", "twitch")
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
                    new KeyValuePair<string, object>("_channel", channelName),
                    new KeyValuePair<string, object>("_guildId", guildId),
                    new KeyValuePair<string, object>("_userid", userid),
                    new KeyValuePair<string, object>("_type", "twitch")
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
                    new KeyValuePair<string, object>("_channel", channelName),
                    new KeyValuePair<string, object>("_ping", ping),
                    new KeyValuePair<string, object>("_discordId", user),
                    new KeyValuePair<string, object>("_guildId", guildid),
                    new KeyValuePair<string, object>("_type", "twitch")
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
                    new KeyValuePair<string, object>("_userId", user),
                    new KeyValuePair<string, object>("_guildId", guildId),
                    new KeyValuePair<string, object>("_ping", ping)
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
                    new KeyValuePair<string, object>("_guildId", guildId),
                    new KeyValuePair<string, object>("_type", "twitch")
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
                    new KeyValuePair<string, object>("_guildId", guildId),
                    new KeyValuePair<string, object>("_type", "twitch")
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
                    new KeyValuePair<string, object>("_guildId", guildId),
                    new KeyValuePair<string, object>("_module", "twitch"),
                    new KeyValuePair<string, object>("_setting", setting)
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
                new KeyValuePair<string, object>("_guildId", guildId),
                new KeyValuePair<string, object>("_module", "twitch"),
                new KeyValuePair<string, object>("_setting", setting)
            };

            args.Add(iValue == default
                ? new KeyValuePair<string, object>("_iValue", DBNull.Value)
                : new KeyValuePair<string, object>("_iValue", iValue));

            args.Add(sValue == string.Empty
                ? new KeyValuePair<string, object>("_sValue", DBNull.Value)
                : new KeyValuePair<string, object>("_sValue", sValue));

            args.Add(biValue == default
                ? new KeyValuePair<string, object>("_biValue", DBNull.Value)
                : new KeyValuePair<string, object>("_biValue", biValue));
            await _mySql.RunProcedure("settings_AddUpdate", args);
        }
    }
}
