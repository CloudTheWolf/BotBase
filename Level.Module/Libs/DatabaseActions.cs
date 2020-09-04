using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using BotLogger;
using Microsoft.Extensions.Logging;

namespace Level.Module.Libs
{
    class DatabaseActions
    {
        public static ILogger<Logger> DbLogger = Level.Logger;
        private static readonly BotData.MySql mSql = new BotData.MySql(DbLogger);


        internal static async Task GiveExp(string member, bool voice = false, decimal expOverride = 0)
        {
            var exp = LevelOptions.ExpPerMessage;
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("userid", member)
            };
            if (voice)
            {
                exp = expOverride;
                args.Add(new KeyValuePair<string, object>("exp", exp.ToString(CultureInfo.InvariantCulture)));
                try
                {
                    await mSql.RunProcedure("Levels_GiveVoiceExp", args);
                }
                catch (Exception e)
                {
                    DbLogger.LogError($"{e}");
                }
                return;
            }

            if (expOverride > 0)
            {
                exp = expOverride;
            }

            args.Add(new KeyValuePair<string, object>("exp", exp.ToString(CultureInfo.InvariantCulture)));
            try
            {
                await mSql.RunProcedure("Levels_GiveMsgExp", args);
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
            }

        }

        internal static async Task RevokeExp(string member, bool voice = false, int expOverride = 0)
        {
            var exp = LevelOptions.ExpPerMessage;
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("userid", member)
            };
            if (voice)
            {
                exp = (expOverride == 0) ? LevelOptions.ExpPerVoiceMin : expOverride;
                args.Add(new KeyValuePair<string, object>("exp", exp.ToString()));
                try
                {
                    await mSql.RunProcedure("Levels_RevokeVoiceExp", args);
                }
                catch (Exception e)
                {
                    DbLogger.LogError($"{e}");
                }
                return;
            }

            if (expOverride > 0)
            {
                exp = expOverride;
            }

            args.Add(new KeyValuePair<string, object>("exp", exp.ToString()));
            try
            {
                await mSql.RunProcedure("Levels_RevokeMsgExp", args);
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
            }

        }

        internal static async Task ResetExp(string member)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("userid", member)
            };
            try
            {
                await Task.Run(() => mSql.RunProcedure("Levels_Reset", args));
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
            }
        }

        internal static async Task SetUserInCall(string member, string tStamp, bool inVoice)
        {
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("userid", member),
                new KeyValuePair<string, object>("timeJoined", tStamp),
                new KeyValuePair<string, object>("inVoice",Convert.ToInt32(inVoice))
            };
            try
            {
                await mSql.RunProcedure("Level_SetVoiceState", args);
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
            }

        }

        internal static async Task<DataTable> GetUserExp(ulong member,bool voice = false)
        {
            Console.WriteLine($"Get Member Exp For Member:{member}");
            var args = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("szUserId", member)
            };
            var spName = voice ? "Level_GetUserVoiceStats" : "Level_GetUserStats";
            try
            {
                //Level_GetUserVoiceStats
                return mSql.RunProcedure(spName, args).Result;
                
            }
            catch (Exception e)
            {
                DbLogger.LogError($"{e}");
                return new DataTable();
            }
        }
    }
}
