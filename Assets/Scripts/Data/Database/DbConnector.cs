using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;
using Mono.Data.SqliteClient;

namespace Data.Database
{
    public partial class DatabaseConnection
    {
        public static DatabaseConnection main;

        public IDataReader reader;
        public IDbConnection dbconn;

        public DatabaseConnection()
        {
            string sqliteConnectionString = "URI=file:" + MacabreDatabaseLocation + ",version=3";

            dbconn = new SqliteConnection(sqliteConnectionString);
            dbconn.Open();
        }

        ~DatabaseConnection()
        {
            if (reader != null) reader.Close();
            if (dbconn != null) dbconn.Close();
            if (reader != null) reader.Dispose();
            if (dbconn != null) dbconn.Dispose();
            reader = null;
            dbconn = null;
        }
        
        public void ExecuteSQL(string query)
        {
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                dbcmd.CommandText = query;
                reader = dbcmd.ExecuteReader();
            }
        }

        static string MacabreDatabaseLocation
        {
			get { return Game.main.saves.current.database; }
        }

        static IDataReader Reader
        {
            get
            {
                if (main == null) main = new DatabaseConnection();
                return main.reader;
            }
        }

        static void ExecuteSQLQuery(string query)
        {
            if (main == null) main = new DatabaseConnection();
            main.ExecuteSQL(query);
        }
    }
}