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
    public partial class DatabaseManager
    {
        private class DatabaseConnection
        {
            public IDataReader reader;
            public IDbConnection dbconn;

            ~DatabaseConnection()
            {
                if (reader != null) reader.Close();
                if (dbconn != null) dbconn.Close();
                if (reader != null) reader.Dispose();
                if (dbconn != null) dbconn.Dispose();
                reader = null;
                dbconn = null;
            }

        }
        static DatabaseConnection connection = new DatabaseConnection();
        static IDataReader reader {
            get {
                return connection.reader;
            }
        }

        // We only use one databse now
        static string MacabreDatabaseLocation
        {
            get { return SaveManager.CurrentSave.databaseLocation; }
        }

        static void StartDatabase()
        {
            string sqliteConnectionString = "URI=file:" + MacabreDatabaseLocation + ",version=3";
             
            connection.dbconn = new SqliteConnection(sqliteConnectionString);
            connection.dbconn.Open();
        }
        
        static void ExecuteSQLQuery(string query)
        {
            //Debug.Log("SQL: " + query);
            if (connection.dbconn == null) StartDatabase();

            using(IDbCommand dbcmd = connection.dbconn.CreateCommand())
            {
                dbcmd.CommandText = query;
                connection.reader = dbcmd.ExecuteReader();
            }
        }
    }
}