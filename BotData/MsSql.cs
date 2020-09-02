using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using BotLogger;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BotData
{
    public class MsSql
    {
        private string dbhost = string.Empty;
        private string dbuser = string.Empty;
        private string dbpass = string.Empty;
        private string dbname = string.Empty;
        public static ILogger<Logger> Logger;
        private static readonly SqlConnection sqlConnection = new SqlConnection();

        public MsSql(ILogger<Logger> logger)
        {
            Logger = logger;

            var json = String.Empty;
            json = File.ReadAllText("config.json");

            dynamic myConfig = JObject.Parse(json);
            dbhost = myConfig["sql"]["host"].ToString();
            dbuser = myConfig["sql"]["user"].ToString();
            dbpass = myConfig["sql"]["pass"].ToString();
            dbname = myConfig["sql"]["name"].ToString();
            
        }


        public static async Task<DataTable> RunStoredProcedureAsync(string procedure,
            List<KeyValuePair<string, object>> arguments)
        {
            try
            {
                SqlCommand command = new SqlCommand(procedure,sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var (key, value) in arguments)
                {
                    command.Parameters.AddWithValue(key, value);
                }
                DataTable dt = new DataTable();
                using (SqlDataAdapter sda = new SqlDataAdapter(command))
                {
                    sda.Fill(dt);
                }

                return dt;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
