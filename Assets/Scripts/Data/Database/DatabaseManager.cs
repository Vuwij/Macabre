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
        static IDbCommand dbcmd;

        // We only use one databse now
        static string MacabreDatabaseLocation
        {
            get { return SaveManager.currentSave.databaseLocation; }
        }

        static void StartDatabase()
        {
            dbconn = (IDbConnection) new SqliteConnection(MacabreDatabaseLocation);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
        }
        
        static void ExecuteSQLQuery(string query)
        {
            Debug.Log("SQL: " + query);
            if (dbcmd == null) StartDatabase();
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
        }
    }
}