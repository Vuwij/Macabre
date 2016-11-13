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
        static IDataReader reader;
        static IDbConnection dbconn;
        
        // We only use one databse now
        static string MacabreDatabaseLocation
        {
            get { return SaveManager.CurrentSave.databaseLocation; }
        }

        static void StartDatabase()
        {
            string sqliteConnectionString = "URI=file:" + MacabreDatabaseLocation + ",version=3";
             
            dbconn = new SqliteConnection(sqliteConnectionString);
            dbconn.Open();
        }
        
        static void ExecuteSQLQuery(string query)
        {
            //Debug.Log("SQL: " + query);
            if (dbconn == null) StartDatabase();

            using(IDbCommand dbcmd = dbconn.CreateCommand())
            {
                dbcmd.CommandText = query;
                reader = dbcmd.ExecuteReader();
            }
        }

        public static void CloseConnections()
        {
            if(reader != null) reader.Close();
            if(dbconn != null) dbconn.Close();
            if(reader != null) reader.Dispose();
            if(dbconn != null) dbconn.Dispose();
            reader = null;
            dbconn = null;
        }
    }
}