using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotLogger;
using Microsoft.Extensions.Logging;

namespace Level.Module.Libs
{
    class DatabaseActions
    {
        public static ILogger<Logger> DbLogger = Level.Logger;
        private static BotData.MySql mSql = new BotData.MySql(DbLogger);


        internal static async Task GiveExp(string member, bool voice = false, int expOverride = 0)
        {
            int exp = LevelOptions.ExpPerMessage;
            var args = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("userid", member)
            };
            if (voice)
            {
                exp = (expOverride == 0) ? LevelOptions.ExpPerVoiceMin : expOverride;
                return;
            }

            if (expOverride > 0)
            {
                exp = expOverride;
            }

            args.Add(new KeyValuePair<string, string>("exp", exp.ToString()));
            try
            {
                await Task.Run(() => mSql.RunProcedure("Levels_GiveMsgExp", args));
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
            }

        }

    }
}
