﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BotLogger;
using Microsoft.Extensions.Logging;

namespace BotData
{
    class MySql
    {
        private string dbhost = string.Empty;
        private string dbuser = string.Empty;
        private string dbpass = string.Empty;
        private string dbname = string.Empty;
        private MySqlConnection sqlConnection = new MySqlConnection();

        public static ILogger<Logger> Logger;
        public MySql(ILogger<Logger> logger)
        {
            Logger = logger;

            var json = String.Empty;
            json = File.ReadAllText("config.json");

            dynamic myConfig = JObject.Parse(json);
            dbhost = myConfig["sql"]["host"].ToString();
            dbuser = myConfig["sql"]["user"].ToString();
            dbpass = myConfig["sql"]["pass"].ToString();
            dbname = myConfig["sql"]["name"].ToString();
            DbConnect();
        }

        private void DbConnect()
        {
            MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder()
            {
                Server = dbhost,
                UserID = dbuser,
                Password = dbpass,
                Database = dbname,
                ConnectionIdlePingTime = 60
            };

            sqlConnection.ConnectionString = cb.ToString();
            try
            {
                sqlConnection.Open();
                Logger.LogInformation("SQL Connection Opened");
                sqlConnection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        public string RunJsonQuery(string query)
        {
            string json = string.Empty;

            try
            {
                DataTable dt = new DataTable();
                using MySqlDataAdapter sda = new MySqlDataAdapter(query, sqlConnection);
                sda.Fill(dt);

                json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                return json;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }

        }

        public DataTable RunDataTableQuery(string query)
        {
            try
            {
                DataTable dt = new DataTable();
                using MySqlDataAdapter sda = new MySqlDataAdapter(query, sqlConnection);
                sda.Fill(dt);

                return dt;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }

            
        }

        [Description("Run a stored procedure and return as a JSON String")]
        public string RunProcedure(string procedure, List<KeyValuePair<string, string>> args)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(procedure, sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var arg in args)
                {
                    command.Parameters.AddWithValue(arg.Key, arg.Value);
                }

                DataTable dt = new DataTable();
                using MySqlDataAdapter sda = new MySqlDataAdapter(command);
                sda.Fill(dt);

                if (dt.Rows.Count == 0)
                    return "";

                return JsonConvert.SerializeObject(dt, Formatting.Indented);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }

        }

        [Description("Run a stored procedure and return as a Data Table")]
        public DataTable RunProcedure(string procedure, List<KeyValuePair<string, object>> arguments)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(procedure, sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var arg in arguments)
                {
                    command.Parameters.AddWithValue(arg.Key, arg.Value);
                }

                DataTable dt = new DataTable();
                using MySqlDataAdapter sda = new MySqlDataAdapter(command);
                sda.Fill(dt);

                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }

        }

    }
}