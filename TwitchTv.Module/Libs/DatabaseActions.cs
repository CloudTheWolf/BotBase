using System;
using System.Data;
using BotLogger;
using Microsoft.Extensions.Logging;

namespace TwitchTv.Module.Libs
{
    class DatabaseActions
    {
        public static ILogger<Logger> DbLogger;
        private static BotData.MySql coveSql;

        internal DatabaseActions()
        {
            DbLogger = Ttv.Logger;
            coveSql = new BotData.MySql(DbLogger);
        }

        internal DataTable GetStreamers()
        {
            try
            {
                return coveSql.RunDataTableQuery(
                    "SELECT * FROM `streams` WHERE lastMessage < DATE_ADD(NOW(), INTERVAL -1 HOUR)");
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
                coveSql.RunDataTableQuery($"UPDATE `streams` SET lastMessage = '{timestamp}' WHERE `id` = {id}");
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal void DeleteStream(string user)
        {
            try
            {
                coveSql.RunDataTableQuery($"DELETE FROM `streams` WHERE `discordId` = {user}");
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal int CanAddStream(ulong user,string channelName)
        {
            try
            {


                var dt = coveSql.RunDataTableQuery(
                    $"SELECT * FROM `streams` WHERE name = '{channelName.ToLower()}' or `discordId`= {user}");
                return dt.Rows.Count;
            }
            catch (Exception e)
            {
                Ttv.Logger.LogCritical(e.Message);
                return 99;
            }
        }

        internal void AddStream(string channelName, ulong user, int ping = 0)
        {
            try
            {
                coveSql.RunDataTableQuery($"INSERT INTO `streams`(`name`, `lastMessage`, `approved`, `discordId`) VALUES ('{channelName.ToLower()}','2000-01-01 00:00','{ping}','{user}')");

            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal void UpgradeStream(ulong user)
        {
            try
            {
                coveSql.RunDataTableQuery($"UPDATE `streams` SET `approved` = '1' WHERE `discordId` = {user}");
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }
        }

        internal string GetAllStreams()
        {
            var returnData = "";
            try
            {
                returnData = coveSql.RunJsonQuery($"SELECT `name` as 'Twitch', `discordId`, CASE WHEN approved = 0 THEN 'No' WHEN approved = 1 THEN 'Yes' END as '@here_Ping' FROM `streams`");
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }

            return returnData;
        }

        internal string GetLastStream()
        {
            var returnData = "";
            try
            {
                returnData = coveSql.RunJsonQuery($"SELECT `name` as 'Twitch', `discordId` FROM `streams` ORDER BY `lastMessage` desc LIMIT 1");
                
            }
            catch (Exception e)
            {
                Ttv.Logger.LogError($"{e.Message}");
            }

            return returnData;
        }
    }
}
